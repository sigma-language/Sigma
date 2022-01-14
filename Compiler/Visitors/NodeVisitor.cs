namespace Compiler.Visitors
{
    using System;
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;

    // <TReturn>
    public abstract class NodeVisitor
    {
        public abstract void Visit(NameNode node);

        public abstract void Visit(NumberNode node);

        public abstract void Visit(BinaryOperationNode node);

        public abstract void Visit(GroupExprNode node);

        public abstract void Visit(SwitchExpressionNode node);

        public abstract void Visit(TernaryNode node);

        public abstract void Visit(PrefixNode node);

        public abstract void Visit(PostfixNode node);

        public abstract void Visit(AssignNode node);

        public abstract void Visit(ExpressionStatementNode node);

        public abstract void Visit(StatementBlockNode node);

        public abstract void Visit(IfNode node);

        public abstract void Visit(WhileNode node);

        public abstract void Visit(PrintNode node);

        public abstract void Visit(VarDeclNode node);

        public abstract void Visit(TypeNode node);
    }
}
