namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Nodes.ExprNodes;
    using Compiler.Visitors;

    public class PrintNode : StatementNode
    {
        public PrintNode(ExprNode expr)
        {
            this.Expr = expr;
        }

        public ExprNode Expr { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
