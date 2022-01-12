namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Nodes.ExprNodes;
    using Compiler.Visitors;

    public class WhileNode : StatementNode
    {
        public WhileNode(ExprNode condition, StatementBlockNode body)
        {
            this.Condition = condition;
            this.Body = body;
        }

        public ExprNode Condition { get; init; }

        public StatementBlockNode Body { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
