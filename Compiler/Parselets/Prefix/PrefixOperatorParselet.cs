namespace Compiler.Parselets.Prefix
{
    using Compiler.Nodes.ExprNodes;

    public class PrefixOperatorParselet : PrefixParselet
    {
        public PrefixOperatorParselet(int precedence)
        {
            this.Precedence = precedence;
        }

        public int Precedence { get; init; }

        public override PrefixNode Parse(Parser parser, Token token)
        {
            // To handle right-associative operators like "^", we allow a slightly
            // lower precedence when parsing the right-hand side. This will let a
            // parselet with the same precedence appear on the right, which will then
            // take *this* parselet's result as its left-hand argument.
            ExprNode right = parser.ParseExpression(this.Precedence);

            return new (token, right);
        }
    }
}
