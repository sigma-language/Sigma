namespace Compiler
{
    using System.Collections.Generic;
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;
    using Compiler.Parselets.Infix;
    using Compiler.Parselets.Prefix;

    public abstract class Parser
    {
        private readonly Dictionary<TokenType, PrefixParselet> prefixParselets;
        private readonly Dictionary<TokenType, InfixParselet> infixParselets;
        private readonly Queue<Token> tokens;

        private Token curToken;
        private Token peekToken;

        public Parser(List<Token> tokens, TextLogger logger)
        {
            this.Logger = logger;
            this.tokens = new (tokens);

            this.prefixParselets = new ();
            this.infixParselets = new ();

            this.NextToken();
            this.NextToken();
        }

        public TextLogger Logger { get; init; }


        public StatementNode Parse()
        {
            //while (!this.CheckToken(TokenType.EOF))
            //{
            //    return this.ParseStatement();
            //}
            return this.ParseStatement();
        }

        public void Match(TokenType kind)
        {
            if (!this.CheckToken(kind))
            {
                this.Logger.Fatal($"Expected {kind}, got {this.curToken.Kind} at {this.curToken.Location}");
            }

            this.NextToken();
        }

        public void Match(TokenType kind, string message)
        {
            if (!this.CheckToken(kind))
            {
                this.Logger.Fatal(message);
            }

            this.NextToken();
        }

        public ExprNode ParseExpression(int precedence = 0)
        {
            var token = this.Consume();

            PrefixParselet prefix;
            if (!this.prefixParselets.TryGetValue(token.Kind, out prefix))
            {
                this.Logger.Fatal($"Unexpected token `{token.Text}` at {token.Location}");
            }

            ExprNode left = prefix.Parse(this, token);

            while (precedence < this.PeekPrecedence())
            {
                token = this.Consume();

                InfixParselet infix = this.infixParselets[token.Kind];
                left = infix.Parse(this, left, token);
            }

            return left;
        }

        protected void Register(TokenType kind, PrefixParselet parselet)
            => this.prefixParselets.Add(kind, parselet);

        protected void Register(TokenType kind, InfixParselet parselet)
            => this.infixParselets.Add(kind, parselet);

        private StatementNode ParseStatement()
        {
            StatementNode statement = this.curToken.Kind switch
            {
                var t when
                    t.IsBuiltinType() => this.ParseVariableDeclaration(),

                TokenType.IF => this.ParseIfStatement(),
                TokenType.WHILE => this.ParseWhileStatement(),
                TokenType.PRINT => this.ParsePrintStatement(),
                TokenType.OPEN_ARROW => this.ParseBlockStatement(),

                _ => this.ParseExpressionStatement()
            };

            if (statement == null)
            {
                this.Logger.Fatal($"Unexpected token `{this.curToken.Text}`");
            }

            return statement;
        }

        private ExpressionStatementNode ParseExpressionStatement()
        {
            ExprNode expr = this.ParseExpression();
            this.Match(TokenType.SEMICOLON, $"[Syntax Error] Missing ';' at {this.curToken.Location} (todo add location)");
            return new (expr);
        }

        private StatementBlockNode ParseBlockStatement()
        {
            List<StatementNode> statements = new ();
            this.Match(TokenType.OPEN_ARROW, $"[Syntax Error] Expected '->' at {this.curToken.Location}");

            while (!this.CheckToken(TokenType.CLOSE_ARROW))
            {
                var startToken = this.curToken;
                statements.Add(this.ParseStatement());

                if (this.curToken == startToken)
                {
                    this.NextToken();
                }
            }

            this.Match(TokenType.CLOSE_ARROW, $"[Syntax Error] Expected '<-' at {this.curToken.Location}");
            return new (statements.ToArray());
        }

        private IfNode ParseIfStatement()
        {
            this.Match(TokenType.IF);

            this.Match(TokenType.LPAREN, $"[Syntax Error] Expected '(' at {this.curToken.Location}");
            var condition = this.ParseExpression();
            this.Match(TokenType.RPAREN, $"[Syntax Error] Expected ')' at {this.curToken.Location}");

            var body = this.ParseBlockStatement();

            return new (condition, body);
        }

        private WhileNode ParseWhileStatement()
        {
            this.Match(TokenType.WHILE);

            this.Match(TokenType.LPAREN, $"[Syntax Error] Expected '(' at {this.curToken.Location}");
            var condition = this.ParseExpression();
            this.Match(TokenType.RPAREN, $"[Syntax Error] Expected ')' at {this.curToken.Location}");

            var body = this.ParseBlockStatement();

            return new (condition, body);
        }

        private PrintNode ParsePrintStatement()
        {
            this.Match(TokenType.PRINT);
            var expr = this.ParseExpression();
            this.Match(TokenType.SEMICOLON, $"[Syntax Error] Missing ';' (todo add location)");

            return new (expr);
        }

        private VarDeclNode ParseVariableDeclaration()
        {
            var typeNode = this.ParseType();

            var id = this.curToken.Text;
            this.Match(TokenType.IDENT, $"[Syntax Error] Expected a valid identifier name, got `{id}` at {this.curToken.Location}");

            ExprNode rhsExpr = null;
            if (this.CheckToken(TokenType.EQ))
            {
                this.Consume();
                rhsExpr = this.ParseExpression();
            }

            this.Match(TokenType.SEMICOLON, $"[Syntax Error] Missing ';' at {this.curToken.Location} (todo add location)");

            return new (typeNode, id, rhsExpr);
        }

        private TypeNode ParseType()
        {
            if (!this.curToken.Kind.IsBuiltinType())
            {
                this.Logger.Fatal($"[Syntax Error] Invalid type `{this.curToken.Text}` at {this.curToken.Location}");
            }

            return new (this.Consume());
        }

        private bool CheckToken(TokenType kind)
            => kind == this.curToken.Kind;

        private bool CheckPeek(TokenType kind)
            => kind == this.peekToken.Kind;

        private Token Consume()
        {
            var token = this.curToken;
            this.NextToken();
            return token;
        }

        private void NextToken()
        {
            this.curToken = this.peekToken;

            if (this.tokens.Count != 0)
            {
                this.peekToken = this.tokens.Dequeue();
            }
        }

        private int PeekPrecedence()
        {
            InfixParselet parselet;
            this.infixParselets.TryGetValue(this.curToken.Kind, out parselet);
            return parselet?.Precedence ?? 0;
        }
    }
}
