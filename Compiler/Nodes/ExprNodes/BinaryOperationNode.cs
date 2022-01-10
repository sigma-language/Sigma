namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class BinaryOperationNode : ExprNode
    {
        public BinaryOperationNode(ExprNode left, Token op, ExprNode right)
        {
            this.Left = left;
            this.Op = op;
            this.Right = right;
        }

        public ExprNode Left { get; init; }

        public Token Op { get; init; }

        public ExprNode Right { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
