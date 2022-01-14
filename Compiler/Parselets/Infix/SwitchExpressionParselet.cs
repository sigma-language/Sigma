namespace Compiler.Parselets.Infix
{
    using System.Collections.Generic;
    using Compiler.Nodes.ExprNodes;

    public class SwitchExpressionParselet : InfixParselet
    {
        public SwitchExpressionParselet(int precedence)
        {
            this.Precedence = precedence;
        }

        public override int Precedence { get; init; }

        public override SwitchExpressionNode Parse(Parser parser, ExprNode controlExpr, Token token)
        {
            parser.Match(TokenType.OPEN_ARROW, $"[Syntax Error] Expected '->' at {token.Location} (todo somewhere here maybe?)");

            List<(BinaryOperationNode, ExprNode)> list = new ();

            while (!parser.CheckToken(TokenType.CLOSE_ARROW))
            {
                Token eqOperatorToken = new ("==", TokenType.EQEQ, -1, -1);
                ExprNode switchCaseExpr = parser.ParseExpression();
                BinaryOperationNode condition = new (controlExpr, eqOperatorToken, switchCaseExpr);

                parser.Match(TokenType.BIG_ARROW);
                var value = parser.ParseExpression();
                parser.Match(TokenType.COMMA);

                list.Add((condition, value));
            }

            parser.Match(TokenType.CLOSE_ARROW, $"[Syntax Error] Expected '<-' at {token.Location} (todo somewhere here maybe?)");

            return new (list.ToArray());
        }
    }
}
