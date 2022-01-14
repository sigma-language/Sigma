namespace Compiler.Visitors
{
    using System.Text;
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;
    using Compiler.Symbols;

    public class CodeGenerator : NodeVisitor
    {
        private readonly StringBuilder result;
        private readonly SymbolTable symbolTable;
        private readonly TextLogger logger;

        public CodeGenerator(SymbolTable symbolTable, TextLogger logger)
        {
            this.result = new ();
            this.symbolTable = symbolTable;
            this.logger = logger;
        }

        public string GenerateCode(dynamic node)
        {
            this.result.Append("#include <stdio.h>\n");
            this.result.Append("#include <stdbool.h>\n");
            this.result.Append("int main(void) {\n");
            this.Visit(node);
            this.result.Append("return 0;\n}");

            return this.result.ToString();
        }

        public override void Visit(NameNode node)
        {
            Symbol _;
            if (!this.symbolTable.LookupSymbol(node.Name, out _))
            {
                this.logger.Fatal($"[Compiler Error] {node.Name} not in symbol table.");
            }

            this.result.Append(node.Name);
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

        public override void Visit(AssignNode node)
        {
            this.Visit((dynamic)node.Name);
            this.result.Append($" = ");
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(ExpressionStatementNode node)
        {
            this.Visit((dynamic)node.Expr);
            this.result.Append(";\n");
        }

        public override void Visit(StatementBlockNode node)
        {
            this.symbolTable.EnterScope();
            this.result.Append("{\n");

            foreach (var statementNode in node.Statements)
            {
                this.Visit((dynamic)statementNode);
            }

            this.result.Append('}');
            this.symbolTable.LeaveScope();
        }

        public override void Visit(IfNode node)
        {
            this.result.Append("if (");
            this.Visit((dynamic)node.Condition);
            this.result.Append(")\n");

            this.Visit((dynamic)node.Body);
            this.result.Append('\n');
        }

        public override void Visit(WhileNode node)
        {
            this.result.Append("while (");
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
            Symbol sym;
            if (!this.symbolTable.LookupSymbol(node.Id, out sym))
            {
                this.logger.Fatal($"[Compiler Error] {node.Id} not in symbol table.");
            }

            VariableSymbol symbol = (VariableSymbol)sym;

            this.Visit((dynamic)node.VarType);
            this.result.Append($"{symbol.Name}");

            if (node.RHS != null)
            {
                this.result.Append($" = ");
                this.Visit((dynamic)node.RHS);
            }

            this.result.Append(";\n");
        }

        public override void Visit(TypeNode node)
        {
            this.result.Append($"{node.Type.Text} ");
        }

        public override void Visit(SwitchExpressionNode node)
        {
            foreach (var c in node.Cases[0.. (node.Cases.Length - 1)])
            {
                this.Visit((dynamic)c.condition);
                this.result.Append($" ? ");
                this.Visit((dynamic)c.value);
                this.result.Append($" : ");
            }

            var lastCase = node.Cases[^1];
            this.Visit((dynamic)lastCase.condition);
            this.result.Append($" ? ");
            this.Visit((dynamic)lastCase.value);
            this.result.Append($" : 0");
        }
    }
}
