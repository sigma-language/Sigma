namespace Compiler.Parselets.Prefix
{
    using Compiler.Nodes.ExprNodes;

    public class GroupParselet : PrefixParselet
    {
        public override ExprNode Parse(Parser parser, Token token)
        {
            ExprNode expression = parser.ParseExpression();
            parser.Match(TokenType.RPAREN);
            return expression;
        }
    }
}
