namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Nodes.ExprNodes;
    using Compiler.Visitors;

    public class IfStatementNode : StatementNode
    {
        public IfStatementNode(ExprNode condition, BlockStatementNode body)
        {
            this.Condition = condition;
            this.Body = body;
        }

        public ExprNode Condition { get; init; }

        public BlockStatementNode Body { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
