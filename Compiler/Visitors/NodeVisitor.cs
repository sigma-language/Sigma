namespace Compiler.Visitors
{
    using System;
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;

    // <TReturn>
    public abstract class NodeVisitor
    {
        public abstract void Visit(NumberNode node);

        public abstract void Visit(BinaryOperationNode node);

        public abstract void Visit(GroupExprNode node);

        public abstract void Visit(TernaryNode node);

        public abstract void Visit(PrefixNode node);

        public abstract void Visit(PostfixNode node);

        public abstract void Visit(StatementBlockNode blockStatementNode);

        public abstract void Visit(IfNode ifStatementNode);

        public abstract void Visit(PrintNode node);

        public abstract void Visit(VarDeclNode node);

        public abstract void Visit(TypeNode node);
    }
}
