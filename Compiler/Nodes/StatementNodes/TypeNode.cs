namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Visitors;

    public class TypeNode : StatementNode
    {
        public TypeNode(Token type)
        {
            this.Type = type;
        }

        public Token Type { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
