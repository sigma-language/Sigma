namespace Compiler.Visitors
{
    using System;
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;

    public abstract class NodeVisitor
    {
        public abstract void Visit(NumberNode node);

        public abstract void Visit(BinaryOperationNode node);

        public abstract void Visit(TernaryNode node);

        public abstract void Visit(PrefixNode node);

        public abstract void Visit(PostfixNode node);

        public abstract void Visit(BlockStatementNode blockStatementNode);

        public abstract void Visit(IfStatementNode ifStatementNode);

        public abstract void Visit(PrintStatementNode node);
    }
}
