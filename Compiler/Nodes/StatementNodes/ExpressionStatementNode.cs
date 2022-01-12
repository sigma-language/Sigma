using System;
namespace Compiler.Nodes.StatementNodes
{
    using Compiler.Nodes.ExprNodes;
    using Compiler.Visitors;

    public class ExpressionStatementNode : StatementNode
    {
        public ExpressionStatementNode(ExprNode expr)
        {
            this.Expr = expr;
        }

        public ExprNode Expr { get; init; }

        public override void Accept(NodeVisitor v)
            => v.Visit(this);
    }
}
