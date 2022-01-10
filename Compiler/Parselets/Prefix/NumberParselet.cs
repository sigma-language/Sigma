namespace Compiler.Parselets.Prefix
{
    using Compiler.Nodes.ExprNodes;

    public class NumberParselet : PrefixParselet
    {
        public override ExprNode Parse(Parser parser, Token token)
        {
            return new NumberNode(token.Text);
        }
    }
}
