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

        private enum XML { start, end, both }

        //All the functions does the same thing:
        //1. Start XML tag of type whatever node type is.
        //2. Accept the node
        //3. End XML tag of whatever node type is.
        //That will generate a XML tree that shows the entire node structure of the program.
        public override object Visit(CallNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Parameters.Accept(this, null);
            return createXmlTag(node, XML.end);
        }
        public override object Visit(VarNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Source?.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(BoolConst node, object o)
        {
            return createXmlTag(node, XML.both);
        }

        public override object Visit(CollecConst node, object o)
        {
            return createXmlTag(node, XML.both);
        }

        public override object Visit(NoneConst node, object o)
        {
            return createXmlTag(node, XML.both);
        }

        public override object Visit(NumConst node, object o)
        {
            return createXmlTag(node, XML.both);
        }

        public override object Visit(TextConst node, object o)
        {
            return createXmlTag(node, XML.both);
        }

        public override object Visit(BinExprNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Left.Accept(this, null);
            node.Right.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(UnaExprNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Expr.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(EdgeCreateNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.LeftSide.Accept(this, null);
            node.RightSide.ForEach(t => { t.Item1.Accept(this, null); t.Item2.ForEach(l => l.Accept(this, null)); });
            return createXmlTag(node, XML.end);
        }

        public override object Visit(FuncDeclNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Parameters.Accept(this, null);
            node.Body.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(VarDeclNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.DefaultValue?.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(VertexDeclNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Attributes.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(AssignNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Target.Accept(this, null);
            node.Value.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(BlockNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.statements.ForEach(n => n.Accept(this, null));
            return createXmlTag(node, XML.end);
        }

        public override object Visit(ForeachNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.IterationVar.Accept(this, null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(ForNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Initializer.Accept(this, null);
            node.Condition.Accept(this, null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(HeadNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.attrDeclBlock.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(IfNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Condition?.Accept(this, null);
            node.Body.Accept(this, null);
            node.ElseNode?.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(LoneCallNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Call.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(ReturnNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Ret.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(WhileNode node, object o)
        {
            createXmlTag(node, XML.start);
            node.Condition.Accept(this, null);
            node.Body.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(MAGIA node, object o)
        {
            createXmlTag(node, XML.start);
            node.block.Accept(this, null);
            return createXmlTag(node, XML.end);
        }

        public override object Visit(BreakNode node, object o)
        {
            return createXmlTag(node, XML.both);
        }

        public override object Visit(ContinueNode node, object o)
        {
            return createXmlTag(node, XML.both);
        }

        public override object Visit(MultiDecl node, object o)
        {
            createXmlTag(node, XML.start);
            node.Decls.ForEach(n => n.Accept(this, null));
            return createXmlTag(node, XML.end);
        }

        private object createXmlTag(Node node, XML state)
        {
            if (state == XML.start || state == XML.both)
                ast.AppendLine($"<{node.GetType().Name}>");
            if (state == XML.end || state == XML.both)
                ast.AppendLine($"</{node.GetType().Name}>");
            return null;
        }
    }
}
