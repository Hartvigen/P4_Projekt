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
        public override object Visit(CallNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Parameters.Accept(this,null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(VarNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(BoolConst node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(CollecConst node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(NoneConst node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(NumConst node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(TextConst node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(BinExprNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Left.Accept(this, null);
            node.Right.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(UnaExprNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Expr.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(EdgeCreateNode node, object o)
        {
            //ast.AppendLine($"<{node.GetType().Name}>");
            //node.Start.Accept(this);
            //node.End.Accept(this);
            //node.Attributes.Accept(this);
            //ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(FuncDeclNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Parameters.Accept(this, null);
            node.Body.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(VarDeclNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            if(node.DefaultValue != null)
            node.DefaultValue.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(VertexDeclNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Attributes.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(AssignNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Target.Accept(this, null);
            node.Value.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(BlockNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            foreach (Node n in node.statements)
                n.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(ForeachNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.IterationVar.Accept(this, null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(ForNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Initializer.Accept(this, null);
            node.Condition.Accept(this, null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(HeadNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.attrDeclBlock.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(IfNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            if(node.Condition != null)
            node.Condition.Accept(this, null);
            node.Body.Accept(this, null);
            if(node.ElseNode != null)
            node.ElseNode.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(LoneCallNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Call.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(ReturnNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Ret.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(WhileNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.Condition.Accept(this, null);
            node.Body.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(MAGIA node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.block.Accept(this, null);
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(BreakNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }

        public override object Visit(ContinueNode node, object o)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }
    }
}
