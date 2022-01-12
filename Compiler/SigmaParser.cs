namespace Compiler
{
    using System.Collections.Generic;
    using Compiler.Parselets.Infix;
    using Compiler.Parselets.Prefix;

    public enum Precedence
    {
        ASSIGNMENT = 1,
        CONDITIONAL,
        BOOL_OR,
        BOOL_AND,
        SUM,
        PRODUCT,
        PREFIX,
        POSTFIX,
        EXPONENT,
    }

    public class SigmaParser : Parser
    {
        public SigmaParser(List<Token> tokens, TextLogger logger)
        : base(tokens, logger)
        {
            this.Register(TokenType.IDENT, new NameParselet());
            this.Register(TokenType.EQ, new AssignParselet((int)Precedence.ASSIGNMENT));
            this.Register(TokenType.NUMBER, new NumberParselet());
            this.Register(TokenType.BIN_NUMBER, new NumberParselet());
            this.Register(TokenType.HEX_NUMBER, new NumberParselet());
            this.Register(TokenType.OCT_NUMBER, new NumberParselet());

            this.Register(TokenType.LPAREN, new GroupParselet());
            this.Register(TokenType.QUESTION, new TernaryParselet((int)Precedence.CONDITIONAL));

            // Register the simple operator parselets.
            this.RegisterPrefix(TokenType.PLUS, (int)Precedence.PREFIX);
            this.RegisterPrefix(TokenType.MINUS, (int)Precedence.PREFIX);

            this.RegisterInfixLeft(TokenType.PLUS, (int)Precedence.SUM);
            this.RegisterInfixLeft(TokenType.MINUS, (int)Precedence.SUM);
            this.RegisterInfixLeft(TokenType.ASTERISK, (int)Precedence.PRODUCT);
            this.RegisterInfixLeft(TokenType.SLASH, (int)Precedence.PRODUCT);

            this.RegisterInfixLeft(TokenType.EQEQ, (int)Precedence.CONDITIONAL);
            this.RegisterInfixLeft(TokenType.NOTEQ, (int)Precedence.CONDITIONAL);
            this.RegisterInfixLeft(TokenType.GT, (int)Precedence.CONDITIONAL);
            this.RegisterInfixLeft(TokenType.LT, (int)Precedence.CONDITIONAL);
            this.RegisterInfixLeft(TokenType.GTEQ, (int)Precedence.CONDITIONAL);
            this.RegisterInfixLeft(TokenType.LTEQ, (int)Precedence.CONDITIONAL);

            this.RegisterInfixLeft(TokenType.ANDAND, (int)Precedence.BOOL_AND);
            this.RegisterInfixLeft(TokenType.OROR, (int)Precedence.BOOL_OR);

            this.RegisterInfixRight(TokenType.CARET, (int)Precedence.EXPONENT);
        }

        private void RegisterPostfix(TokenType kind, int precedence)
        {
            this.Register(kind, new PostfixOperatorParselet(precedence));
        }

        private void RegisterPrefix(TokenType kind, int precedence)
        {
            this.Register(kind, new PrefixOperatorParselet(precedence));
        }

        private void RegisterInfixLeft(TokenType kind, int precedence)
        {
            this.Register(kind, new BinaryOperatorParselet(precedence, false));
        }

        private void RegisterInfixRight(TokenType kind, int precedence)
        {
            this.Register(kind, new BinaryOperatorParselet(precedence, true));
        }
    }
}
