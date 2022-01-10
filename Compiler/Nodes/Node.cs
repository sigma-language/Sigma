namespace Compiler.Nodes
{
    using Compiler.Visitors;

    public abstract class Node
    {

        public abstract void Accept(NodeVisitor v);
    }
}
