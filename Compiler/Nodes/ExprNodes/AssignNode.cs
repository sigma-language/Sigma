namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class AssignNode : ExprNode
    {
        public AssignNode(NameNode name, ExprNode right)
        {
            this.Name = name;
            this.Right = right;
        }

        public NameNode Name { get; init; }

        public ExprNode Right { get; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
