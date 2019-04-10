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
        public override void Visit(CallNode node)
        {
            createXmlTag(node, XML.start);
            node.Parameters.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(VarNode node)
        {
            createXmlTag(node, XML.start);
            node.Source?.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(BoolConst node)
        {
            createXmlTag(node, XML.both);
        }

        public override void Visit(CollecConst node)
        {
            createXmlTag(node, XML.both);
        }

        public override void Visit(NoneConst node)
        {
            createXmlTag(node, XML.both);
        }

        public override void Visit(NumConst node)
        {
            createXmlTag(node, XML.both);
        }

        public override void Visit(TextConst node)
        {
            createXmlTag(node, XML.both);
        }

        public override void Visit(BinExprNode node)
        {
            createXmlTag(node, XML.start);
            node.Left.Accept(this);
            node.Right.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(UnaExprNode node)
        {
            createXmlTag(node, XML.start);
            node.Expr.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(EdgeCreateNode node)
        {
            createXmlTag(node, XML.start);
            node.LeftSide.Accept(this);
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });
            createXmlTag(node, XML.end);
        }

        public override void Visit(FuncDeclNode node)
        {
            createXmlTag(node, XML.start);
            node.Parameters.Accept(this);
            node.Body.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(VarDeclNode node)
        {
            createXmlTag(node, XML.start);
            node.DefaultValue?.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(VertexDeclNode node)
        {
            createXmlTag(node, XML.start);
            node.Attributes.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(AssignNode node)
        {
            createXmlTag(node, XML.start);
            node.Target.Accept(this);
            node.Value.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(BlockNode node)
        {
            createXmlTag(node, XML.start);
            node.statements.ForEach(n => n.Accept(this));
            createXmlTag(node, XML.end);
        }

        public override void Visit(ForeachNode node)
        {
            createXmlTag(node, XML.start);
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(ForNode node)
        {
            createXmlTag(node, XML.start);
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(HeadNode node)
        {
            createXmlTag(node, XML.start);
            node.attrDeclBlock.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(IfNode node)
        {
            createXmlTag(node, XML.start);
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.ElseNode?.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(LoneCallNode node)
        {
            createXmlTag(node, XML.start);
            node.Call.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(ReturnNode node)
        {
            createXmlTag(node, XML.start);
            node.Ret.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(WhileNode node)
        {
            createXmlTag(node, XML.start);
            node.Condition.Accept(this);
            node.Body.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(MAGIA node)
        {
            createXmlTag(node, XML.start);
            node.block.Accept(this);
            createXmlTag(node, XML.end);
        }

        public override void Visit(BreakNode node)
        {
            createXmlTag(node, XML.both);
        }

        public override void Visit(ContinueNode node)
        {
            createXmlTag(node, XML.both);
        }

        public override void Visit(MultiDecl node)
        {
            createXmlTag(node, XML.start);
            node.Decls.ForEach(n => n.Accept(this));
            createXmlTag(node, XML.end);
        }

        private void createXmlTag(Node node, XML state)
        {
            if (state == XML.start || state == XML.both)
                ast.AppendLine($"<{node.GetType().Name}>");
            if(state == XML.end || state == XML.both)
                ast.AppendLine($"</{node.GetType().Name}>");
        }
    }
}
