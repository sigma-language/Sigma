namespace Compiler.Parselets.Infix
{
    using Compiler.Nodes.ExprNodes;

    public class TernaryParselet : InfixParselet
    {
        public TernaryParselet(int precedence)
        {
            this.Precedence = precedence;
        }

        public override int Precedence { get; init; }

        public override TernaryNode Parse(Parser parser, ExprNode condition, Token token)
        {
            ExprNode thenArm = parser.ParseExpression();
            parser.Match(TokenType.COLON);
            ExprNode elseArm = parser.ParseExpression(this.Precedence - 1);

            return new (condition, thenArm, elseArm);
        }
    }
}
