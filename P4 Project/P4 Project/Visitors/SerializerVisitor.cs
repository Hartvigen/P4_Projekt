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
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(IdentNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(MemberNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VarNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BoolConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(CollecConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(NoneConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(NumConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(TextConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BinExprNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(UnaExprNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(EdgeDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(FuncDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VarDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VEDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VertexDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(AssignNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(Block node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            foreach (Node n in node.commands)
                n.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForeachNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(HeadNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.attrDeclBlock.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(IfNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(LoneCallNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ReturnNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
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
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(MAGIA node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.block.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }
    }
}
