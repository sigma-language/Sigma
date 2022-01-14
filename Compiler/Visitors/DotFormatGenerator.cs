﻿namespace Compiler.Visitors
{
    using System.Collections.Generic;
    using System.Text;
    using Compiler.Nodes.ExprNodes;
    using Compiler.Nodes.StatementNodes;

    public class DotFormatGenerator : NodeVisitor
    {
        private readonly StringBuilder graphCode;

        private int parentId;
        private int counter;

        public DotFormatGenerator()
        {
            this.graphCode = new ();
        }

        public string GenerateDotString(dynamic node)
        {
            this.graphCode.Append("digraph AST {\n");

            this.Visit(node);

            this.graphCode.Append('}');

            return this.graphCode.ToString();
        }

        public override void Visit(NameNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=\"{node.Name}\"];\n");
        }

        public override void Visit(NumberNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=\"{node.Value}\"];\n");
        }

        public override void Visit(BinaryOperationNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=\"{node.Op.Text}\"];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Left);

            this.parentId = selfId;
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(GroupExprNode node)
        {
            this.Visit((dynamic)node.Expr);
        }

        public override void Visit(TernaryNode node)
        {
            var selfId = this.counter;
            this.counter++;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=ternary shape=box];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Condition);

            this.parentId = selfId;
            this.Visit((dynamic)node.ThenArm);

            this.parentId = selfId;
            this.Visit((dynamic)node.ElseArm);
        }

        public override void Visit(SwitchExpressionNode node)
        {
            var selfId = this.counter;
            this.counter++;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=switchExpr shape=box];\n");

            int i = 999999;
            foreach (var c in node.Cases)
            {
                var nodeId = selfId + i;
                this.graphCode.Append($"n{nodeId} [label=case];\n");
                this.graphCode.Append($"n{selfId} -> n{nodeId};\n");

                this.parentId = nodeId;
                this.Visit((dynamic)c.condition);

                this.parentId = nodeId;
                this.Visit((dynamic)c.value);
                i += 1;
            }
        }

        public override void Visit(PrefixNode node)
        {
            var selfId = this.counter;
            this.counter++;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=\"{node.Op.Text}\"];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(PostfixNode node)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(AssignNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=\"=\"];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Name);

            this.parentId = selfId;
            this.Visit((dynamic)node.Right);
        }

        public override void Visit(ExpressionStatementNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=Expr shape=box];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Expr);
        }

        public override void Visit(StatementBlockNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=body shape=box];\n");

            foreach (var statement in node.Statements)
            {
                this.parentId = selfId;
                this.Visit((dynamic)statement);
            }
        }

        public override void Visit(IfNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=if shape=box];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Condition);

            this.parentId = selfId;
            this.Visit((dynamic)node.Body);
        }

        public override void Visit(WhileNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=while shape=box];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Condition);

            this.parentId = selfId;
            this.Visit((dynamic)node.Body);
        }

        public override void Visit(PrintNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=print shape=box];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.Expr);
        }

        public override void Visit(VarDeclNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label=\"VarDecl:\\n{node.VarType.Type.Text} {node.Id}\" shape=box];\n");

            this.parentId = selfId;
            this.Visit((dynamic)node.VarType);

            if (node.RHS != null)
            {
                this.parentId = selfId;
                this.Visit((dynamic)node.RHS);
            }
        }

        public override void Visit(TypeNode node)
        {
            var selfId = this.counter;
            this.counter += 1;

            if (this.parentId != selfId)
            {
                this.graphCode.Append($"n{this.parentId} -> n{selfId};\n");
            }

            this.graphCode.Append($"n{selfId} [label={node.Type.Text} shape=box];\n");
        }
    }
}
