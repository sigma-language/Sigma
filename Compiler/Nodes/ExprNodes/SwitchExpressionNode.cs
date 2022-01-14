namespace Compiler.Nodes.ExprNodes
{
    using Compiler.Visitors;

    public class SwitchExpressionNode : ExprNode
    {
        public SwitchExpressionNode((BinaryOperationNode, ExprNode)[] cases)
        {
            this.Cases = cases;
        }

        public (BinaryOperationNode condition, ExprNode value)[] Cases { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
