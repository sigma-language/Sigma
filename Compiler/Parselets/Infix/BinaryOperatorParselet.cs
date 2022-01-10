namespace Compiler.Parselets.Infix
{
    using Compiler.Nodes.ExprNodes;

    public class BinaryOperatorParselet : InfixParselet
    {
        public BinaryOperatorParselet(int precedence, bool isRight)
        {
            this.Precedence = precedence;
            this.IsRight = isRight;
        }

        public override int Precedence { get; init; }

        public bool IsRight { get; }

        public override BinaryOperationNode Parse(Parser parser, ExprNode left, Token token)
        {
            // To handle right-associative operators like "^", we allow a slightly
            // lower precedence when parsing the right-hand side. This will let a
            // parselet with the same precedence appear on the right, which will then
            // take *this* parselet's result as its left-hand argument.
            ExprNode right = parser.ParseExpression(this.Precedence - (this.IsRight ? 1 : 0));

            return new (left, token, right);
        }
    }
}
