namespace Compiler.Parselets.Infix
{
    using Compiler.Nodes.ExprNodes;

    public class PostfixOperatorParselet : InfixParselet
    {
        public PostfixOperatorParselet(int precedence)
        {
            this.Precedence = precedence;
        }

        public override int Precedence { get; init; }

        public override PostfixNode Parse(Parser parser, ExprNode left, Token token)
        {
            return new (left, token);
        }
    }
}
