namespace Compiler.Parselets.Prefix
{
    using Compiler.Nodes.ExprNodes;

    public abstract class PrefixParselet
    {
        public abstract ExprNode Parse(Parser parser, Token token);
    }
}
