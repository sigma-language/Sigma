namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Nodes.ExprNodes;
    using Compiler.Visitors;

    public class VarDeclNode : StatementNode
    {
        public VarDeclNode(TypeNode typeNode, string id, ExprNode rhsExpr = null)
        {
            this.VarType = typeNode;
            this.Id = id;
            this.RHS = rhsExpr;
        }

        public TypeNode VarType { get; init; }

        public string Id { get; init; }

        public ExprNode RHS { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
