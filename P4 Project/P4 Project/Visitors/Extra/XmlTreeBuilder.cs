using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.SymbolTable;

namespace P4_Project.Visitors
{
    public class XmlTreeBuilder : Visitor
    {
        public override string AppropriateFileName { get; } = "xmlTree.xml";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public override SymTable Table { get; set; }

        private enum Xml { START, END, BOTH }

        //All the functions does the same thing:
        //1. Start XML tag of type whatever node type is.
        //2. Accept the node
        //3. End XML tag of whatever node type is.
        //That will generate a XML tree that shows the entire node structure of the program.
        public override void Visit(CallNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Parameters.Accept(this);
            CreateXmlTag(node, Xml.END);
        }
        public override void Visit(VarNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Source?.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(BoolConst node)
        {
            CreateXmlTag(node, Xml.BOTH);
        }

        public override void Visit(CollecConst node)
        {
            CreateXmlTag(node, Xml.BOTH);
        }

        public override void Visit(NoneConst node)
        {
            CreateXmlTag(node, Xml.BOTH);
        }

        public override void Visit(NumConst node)
        {
            CreateXmlTag(node, Xml.BOTH);
        }

        public override void Visit(TextConst node)
        {
            CreateXmlTag(node, Xml.BOTH);
        }

        public override void Visit(BinExprNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Left.Accept(this);
            node.Right.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(UnaExprNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Expr.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(EdgeCreateNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.LeftSide.Accept(this);
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(FuncDeclNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Parameters.Accept(this);
            node.Body.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(VarDeclNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.DefaultValue?.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(VertexDeclNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Attributes.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(AssignNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Target.Accept(this);
            node.Value.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(BlockNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Statements.ForEach(n => n.Accept(this));
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(ForeachNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(ForNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(HeadNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.attrDeclBlock.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(IfNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.ElseNode?.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(LoneCallNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Call.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(ReturnNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Ret.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(WhileNode node)
        {
            CreateXmlTag(node, Xml.START);
            node.Condition.Accept(this);
            node.Body.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(Magia node)
        {
            CreateXmlTag(node, Xml.START);
            node.block.Accept(this);
            CreateXmlTag(node, Xml.END);
        }

        public override void Visit(BreakNode node)
        {
            CreateXmlTag(node, Xml.BOTH);
        }

        public override void Visit(ContinueNode node)
        {
            CreateXmlTag(node, Xml.BOTH);
        }

        public override void Visit(MultiDecl node)
        {
            CreateXmlTag(node, Xml.START);
            node.Decls.ForEach(n => n.Accept(this));
            CreateXmlTag(node, Xml.END);
        }

        private void CreateXmlTag(Node node, Xml state)
        {
            if (state == Xml.START || state == Xml.BOTH)
                Result.AppendLine($"<{node.GetType().Name}>");
            if (state == Xml.END || state == Xml.BOTH)
                Result.AppendLine($"</{node.GetType().Name}>");
        }
    }
}
