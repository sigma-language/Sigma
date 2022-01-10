namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class NumberNode : ExprNode
    {
        public NumberNode(string value)
        {
            this.Value = value;
        }

        public string Value { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
