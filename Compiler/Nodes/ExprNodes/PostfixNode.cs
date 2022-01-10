namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class PostfixNode : ExprNode
    {
        public PostfixNode(ExprNode left, Token op)
        {
            this.Left = left;
            this.Op = op;
        }

        public ExprNode Left { get; init; }

        public Token Op { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
