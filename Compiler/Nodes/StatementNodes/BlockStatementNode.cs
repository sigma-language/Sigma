namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Visitors;

    public class BlockStatementNode : StatementNode
    {
        public BlockStatementNode(StatementNode[] statements)
        {
            this.Statements = statements;
        }

        public StatementNode[] Statements { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
