namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class GroupExprNode : ExprNode
    {
        public GroupExprNode(ExprNode expr)
        {
            this.Expr = expr;
        }

        public ExprNode Expr { get; init; }

        public override void Accept(NodeVisitor v)
        {
            throw new System.NotImplementedException();
        }
    }
}
