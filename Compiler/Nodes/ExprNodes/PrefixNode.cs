namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class PrefixNode : ExprNode
    {
        public PrefixNode(Token op, ExprNode right)
        {
            this.Op = op;
            this.Right = right;
        }

        public Token Op { get; init; }

        public ExprNode Right { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
