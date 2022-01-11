namespace Compiler.Visitors
{
    using System.Text;
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;

    public class CodeGenerator : NodeVisitor
    {
        private readonly StringBuilder result;

        public CodeGenerator()
        {
            this.result = new ();
        }

        public string GenerateCode(dynamic node)
        {
            this.result.Append("#include <stdio.h>\n");
            this.result.Append("int main(void) {\n");
            this.Visit(node);
            this.result.Append("return 0;\n}");

            return this.result.ToString();
        }

        public override void Visit(NumberNode node)
        {
            this.result.Append(node.Value);
        }

        public override void Visit(BinaryOperationNode node)
        {
            this.Visit((dynamic)node.Left);
            this.result.Append(node.Op.Text);
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(GroupExprNode node)
        {
            this.result.Append('(');
            this.Visit((dynamic)node.Expr);
            this.result.Append(')');
        }

        public override void Visit(TernaryNode node)
        {
            this.result.Append('(');
            this.Visit((dynamic)node.Condition);
            this.result.Append(" ? ");
            this.Visit((dynamic)node.ThenArm);
            this.result.Append(" : ");
            this.Visit((dynamic)node.ElseArm);
            this.result.Append(')');
        }

        public override void Visit(PrefixNode node)
        {
            this.result.Append(node.Op.Text);
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(PostfixNode node)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(StatementBlockNode node)
        {
            this.result.Append("{\n");

            foreach (StatementNode statementNode in node.Statements)
            {
                this.Visit((dynamic)statementNode);
            }

            this.result.Append('}');
        }

        public override void Visit(IfNode node)
        {
            this.result.Append("if (");
            this.Visit((dynamic)node.Condition);
            this.result.Append(")\n");

            this.Visit((dynamic)node.Body);
            this.result.Append('\n');
        }

        public override void Visit(PrintNode node)
        {
            this.result.Append("printf(\"%d\\n\", ");
            this.Visit((dynamic)node.Expr);
            this.result.Append(");\n");
        }

        public override void Visit(VarDeclNode node)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(TypeNode node)
        {
            throw new System.NotImplementedException();
        }
    }
}
