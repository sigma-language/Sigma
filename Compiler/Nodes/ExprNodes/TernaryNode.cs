namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class TernaryNode : ExprNode
    {
        public TernaryNode(ExprNode condition, ExprNode thenArm, ExprNode elseArm)
        {
            this.Condition = condition;
            this.ThenArm = thenArm;
            this.ElseArm = elseArm;
        }

        public ExprNode Condition { get; }

        public ExprNode ThenArm { get; }

        public ExprNode ElseArm { get; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
