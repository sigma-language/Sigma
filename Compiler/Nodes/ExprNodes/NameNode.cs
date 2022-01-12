namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class NameNode : ExprNode
    {
        public NameNode(Token id)
        {
            this.Id = id;
        }

        public Token Id { get; init; }

        public string Name => this.Id.Text;

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
