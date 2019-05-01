using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Types;
using P4_Project.Types.Primitives;

namespace P4_Project.Visitors
{
    public class XmlTreeBuilder : Visitor
    {
        public override string AppropriateFileName { get; } = "xmlTree.xml";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; }

        private enum Xml { START, END, BOTH }

        //All the functions does the same thing:
        //1. Start XML tag of type whatever node type is.
        //2. Accept the node
        //3. End XML tag of whatever node type is.
        //That will generate a XML tree that shows the entire node structure of the program.
        public override BaseType Visit(CallNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Parameters.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }
        public override BaseType Visit(VarNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Source?.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(BoolConst node)
        {
            return CreateXmlTag(node, Xml.BOTH);
        }

        public override BaseType Visit(CollecConst node)
        {
            return CreateXmlTag(node, Xml.BOTH);
        }

        public override BaseType Visit(NoneConst node)
        {
            return CreateXmlTag(node, Xml.BOTH);
        }

        public override BaseType Visit(NumConst node)
        {
            return CreateXmlTag(node, Xml.BOTH);
        }

        public override BaseType Visit(TextConst node)
        {
            return CreateXmlTag(node, Xml.BOTH);
        }

        public override BaseType Visit(BinExprNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Left.Accept(this);
            node.Right.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(UnaExprNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Expr.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(EdgeCreateNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.LeftSide.Accept(this);
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(FuncDeclNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Parameters.Accept(this);
            node.Body.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(VarDeclNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.DefaultValue?.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(VertexDeclNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Attributes.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(AssignNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Target.Accept(this);
            node.Value.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(BlockNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Statements.ForEach(n => n.Accept(this));
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(ForeachNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(ForNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(HeadNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.attrDeclBlock.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(IfNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.ElseNode?.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(LoneCallNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Call.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(ReturnNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Ret.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(WhileNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Condition.Accept(this);
            node.Body.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(Magia node)
        {
            CreateXmlTag(node, Xml.START);
            node.block.Accept(this);
            return CreateXmlTag(node, Xml.END);
        }

        public override BaseType Visit(BreakNode node)
        {
            return CreateXmlTag(node, Xml.BOTH);
        }

        public override BaseType Visit(ContinueNode node)
        {
            return CreateXmlTag(node, Xml.BOTH);
        }

        public override BaseType Visit(MultiDecl node)
        {
            CreateXmlTag(node, Xml.START);
            node.Decls.ForEach(n => n.Accept(this));
            return CreateXmlTag(node, Xml.END);
        }

        private BaseType CreateXmlTag(Node node, Xml state)
        {
            if (state == Xml.START || state == Xml.BOTH)
                Result.AppendLine($"<{node.GetType().Name}>");
            if (state == Xml.END || state == Xml.BOTH)
                Result.AppendLine($"</{node.GetType().Name}>");
            return new NoneType();
        }
    }
}
