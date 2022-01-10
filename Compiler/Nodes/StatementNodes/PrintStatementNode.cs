namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Nodes.ExprNodes;
    using Compiler.Visitors;

    public class PrintStatementNode : StatementNode
    {
        public PrintStatementNode(ExprNode expr)
        {
            this.Expr = expr;
        }

        public ExprNode Expr { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
