namespace Compiler.Visitors
{
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;
    using Compiler.Symbols;

    public class SymbolResolver : NodeVisitor
    {
        private readonly SymbolTable symbolTable;
        private readonly TextLogger logger;

        public SymbolResolver(TextLogger logger)
        {
            this.logger = logger;
            this.symbolTable = new ();
        }

        public SymbolTable GenerateSymbolTable(dynamic node)
        {
            this.Visit((dynamic)node);
            return this.symbolTable;
        }

        public override void Visit(NameNode node)
        {
            Symbol symbol;
            if (!this.symbolTable.LookupSymbol(node.Name, out symbol))
            {
                this.logger.Fatal($"[Error] The name `{node.Name}` does not exist in the current context");
            }
        }

        public override void Visit(NumberNode node)
        {
            return;
        }

        public override void Visit(BinaryOperationNode node)
        {
            this.Visit((dynamic)node.Left);
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(GroupExprNode node)
        {
            this.Visit((dynamic)node.Expr);
        }

        public override void Visit(TernaryNode node)
        {
            this.Visit((dynamic)node.Condition);
            this.Visit((dynamic)node.ThenArm);
            this.Visit((dynamic)node.ElseArm);
        }

        public override void Visit(PrefixNode node)
        {
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(PostfixNode node)
        {
            this.Visit((dynamic)node.Left);
        }

        public override void Visit(AssignNode node)
        {
            this.Visit((dynamic)node.Name);
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(ExpressionStatementNode node)
        {
            this.Visit((dynamic)node.Expr);
        }

        public override void Visit(StatementBlockNode node)
        {
            this.symbolTable.EnterScope();
            foreach (var statementNode in node.Statements)
            {
                this.Visit((dynamic)statementNode);
            }

            this.symbolTable.LeaveScope();
        }

        public override void Visit(IfNode node)
        {
            this.Visit((dynamic)node.Condition);
            this.Visit((dynamic)node.Body);
        }

        public override void Visit(WhileNode node)
        {
            this.Visit((dynamic)node.Condition);
            this.Visit((dynamic)node.Body);
        }

        public override void Visit(PrintNode node)
        {
            this.Visit((dynamic)node.Expr);
        }

        public override void Visit(VarDeclNode node)
        {
            if (!this.symbolTable.InsertSymbol(node.Id, new VariableSymbol(node.Id, node.Type)))
            {
                this.logger.Fatal($"[Error] A local variable or function `{node.Id}` is already declared in this scope");
            }
        }

        public override void Visit(TypeNode node)
        {
            return;
        }

        public override void Visit(SwitchExpressionNode node)
        {
            return;
        }
    }
}
