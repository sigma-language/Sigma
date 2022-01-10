namespace Compiler.Parselets.Infix
{
    using Compiler.Nodes.ExprNodes;

    public abstract class InfixParselet
    {
        public abstract int Precedence { get; init; }

        public abstract ExprNode Parse(Parser parser, ExprNode left, Token token);
    }
}
