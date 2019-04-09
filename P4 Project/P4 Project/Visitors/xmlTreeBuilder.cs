using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Visitors
{
    class XmlTreeBuilder : Visitor
    {
        public StringBuilder ast = new StringBuilder();

        //All the functions does the same thing:
        //1. Start XML tag of type whatever node type is.
        //2. Accept the node
        //3. End XML tag of whatever node type is.
        //That will generate a XML tree that shows the entire node structure of the program.
        public override void Visit(CallNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Parameters.Accept(this);
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
            node.Left.Accept(this);
            node.Right.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(UnaExprNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Expr.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(EdgeCreateNode node)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            //node.Start.Accept(this);
            //node.End.Accept(this);
            //node.Attributes.Accept(this);
            //ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(FuncDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Parameters.Accept(this);
            node.Body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VarDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            if(node.DefaultValue != null)
            node.DefaultValue.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VertexDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Attributes.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(AssignNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Target.Accept(this);
            node.Value.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BlockNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            foreach (Node n in node.statements)
                n.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForeachNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
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
            if(node.Condition != null)
            node.Condition.Accept(this);
            node.Body.Accept(this);
            if(node.ElseNode != null)
            node.ElseNode.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(LoneCallNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Call.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ReturnNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Ret.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(WhileNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Condition.Accept(this);
            node.Body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(MAGIA node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.block.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BreakNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ContinueNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }
    }
}
