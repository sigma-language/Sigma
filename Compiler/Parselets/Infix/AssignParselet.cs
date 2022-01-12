namespace Compiler.Parselets.Infix
{
    using Compiler.Nodes.ExprNodes;

    public class AssignParselet : InfixParselet
    {
        public AssignParselet(int precedence)
        {
            this.Precedence = precedence;
        }

        public override int Precedence { get; init; }

        public override AssignNode Parse(Parser parser, ExprNode left, Token token)
        {
            ExprNode right = parser.ParseExpression(this.Precedence - 1);

            if (!(left.GetType() == typeof(NameNode)))
            {
                parser.Logger.Fatal($"[Syntax Error] The left-hand side of an assignment must be a variable");
            }

            return new ((NameNode)left, right);
        }
    }
}
