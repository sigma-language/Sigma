namespace Compiler.Parselets.Prefix
{
    using Compiler.Nodes.ExprNodes;

    public class NameParselet : PrefixParselet
    {
        public override NameNode Parse(Parser parser, Token token)
        {
            return new (token);
        }
    }
}
