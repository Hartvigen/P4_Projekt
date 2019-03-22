using System;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Visitors
{
    class SerializerVisitor : Visitor
    {
        public StringBuilder ast = new StringBuilder();


        public override void Visit(CallNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append(node.identifier + "(");
            node.parameters.Accept(this);
            ast.Append(")");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(IdentNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine(node.identifier);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(MemberNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.source.Accept(this);
            node.memberIdent.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VarNode node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append(node.identifier);
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BoolConst node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append(node.getString());
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(CollecConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            foreach (Node n in node.exprs)
            {
                n.Accept(this);
                ast.Append(", ");
            }
            ast.Remove(ast.Length - 2, 1); //Remove the last comma.
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(NoneConst node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append("none");
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(NumConst node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append(node.getString());
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(TextConst node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            if (node.value == null)
                ast.Append("");
            else ast.Append(node.value);
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BinExprNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.left.Accept(this);
            ast.Append(node.getCodeofOperator());
            node.right.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(UnaExprNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append(node.getCodeofOperator());
            node.expr.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(EdgeDeclNode node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            node.start.Accept(this);
            ast.Append(node.getCodeofOperator());

            if (node.attributes.statements.Count != 0)
                ast.Append("(");

            node.end.Accept(this);

            if (node.attributes.statements.Count != 0)
                ast.Append(", ");

            node.attributes.Accept(this);

            if (node.attributes.statements.Count != 0)
                ast.Append(")");

            ast.AppendLine("");

            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(FuncDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.parameters.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VarDeclNode node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append(node.getVarType() + " " + node.symbolName);
            if (node.expr != null)
            {
                ast.Append(" = ");
                node.expr.Accept(this);
            }
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VEDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VertexDeclNode node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");

            if (node.attributes.statements.Count > 0)
                ast.Append("vertex{");
            else ast.Append("vertex(");

            if (node.attributes.statements.Count > 0)
                ast.Append("(");

            ast.Append(node.symbolName);
            if (node.attributes.statements.Count > 0)
                ast.Append(", ");
            node.attributes.Accept(this);

            if (node.attributes.statements.Count > 0)
                ast.Append(")");

            if (node.attributes.statements.Count > 0)
                ast.Append("}");
            else ast.Append(")");

            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(AssignNode node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            node.target.Accept(this);
            ast.Append(" = ");
            node.value.Accept(this);
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(Block node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");

            foreach(Node n in node.statements)
                n.Accept(this);

            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForeachNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.iterationVar.Accept(this);
            node.iterator.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.initializer.Accept(this);
            node.condition.Accept(this);
            node.iterator.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(HeadNode node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            ast.Append("[" + node.getName() + "(");
            foreach (Node n in node.attrDeclBlock.statements)
            {
                n.Accept(this);
                ast.Append(", ");
            }

            ast.Remove(ast.Length - 2, 1); //Remove the last comma.

            ast.AppendLine(")]");
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(IfNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.condition.Accept(this);
            node.body.Accept(this);
            if (node.elseNode != null)
                node.elseNode.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(LoneCallNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.call.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ReturnNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.ret.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(StmtNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(WhileNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.condition.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(MAGIA node)
        {
            ast.AppendLine("<?xml version=\"1.0\"?>");
            ast.AppendLine($"<{node.GetType().Name}>");
            node.block.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }
    }
}
