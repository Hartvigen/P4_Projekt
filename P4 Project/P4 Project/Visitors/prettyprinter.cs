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
    public class PrettyPrinter : Visitor
    {
        //The string that contains the prettyprinted code.
        public StringBuilder str = new StringBuilder();

        //The level of ident is changed as the visitor moves along so this should start at 0.
        //Changing this will not change the relative indentations.
        //Should never be negative.
        private int indentLevel = 0;

        //The Desired indent size can be changed here 4 is default.
        private int indentSizeInSpaces = 4;


        //Below is function that handles each kind of node and prints it pretty complete with
        //spaces, indentation, brackets, spaces and so on...
        public override object Visit(CallNode node, object o)
        {
            str.Append(node.Identifier + "(");
            node.Parameters.Accept(this, null);
            str.Append(")");

            return null;
        }

        public override object Visit(VarNode node, object o)
        {
            str.Append(node.Identifier);
            return null;
        }

        public override object Visit(BoolConst node, object o)
        {
            str.Append(node.GetString());
            return null;
        }

        public override object Visit(CollecConst node, object o)
        {
            if (node.Expressions.Count == 0)
                return null;
            foreach (Node n in node.Expressions)
            {
                n.Accept(this, null);
                str.Append(", ");
            }
            RemoveLastCommaAndSpace();
            return null;
        }

        public override object Visit(NoneConst node, object o)
        {
            str.Append("none");
            return null;
        }

        public override object Visit(NumConst node, object o)
        {
            str.Append(node.GetString());
            return null;
        }

        public override object Visit(TextConst node, object o)
        {
            str.Append(node.Value);
            return null;
        }

        public override object Visit(BinExprNode node, object o)
        {
            if (node.InParentheses)
                str.Append("(");
            node.Left.Accept(this, null);
            str.Append(" " + node.GetCodeofOperator() + " ");
            node.Right.Accept(this, null);
            if (node.InParentheses)
                str.Append(")");
            return null;
        }

        public override object Visit(UnaExprNode node, object o)
        {
            if (node.InParentheses)
                str.Append("(");
            str.Append(node.GetCodeofOperator());
            node.Expr.Accept(this, null);
            if (node.InParentheses)
                str.Append("(");
            return null;
        }

        public override object Visit(EdgeCreateNode node, object o)
        {
            //node.Start.Accept(this);
            //str.Append(node.GetCodeofOperator());
            //if (node.Attributes.statements.Count != 0)
            //    str.Append("(");
            //node.End.Accept(this);
            //if (node.Attributes.statements.Count != 0)
            //    str.Append(", ");
            //foreach (Node n in node.Attributes.statements)
            //{
            //    n.Accept(this);
            //    RemoveIndentAndNewline();
            //    str.Append(", ");
            //}
            //if(node.Attributes.statements.Count > 0)
            //RemoveLastCommaAndSpace();
            //if (node.Attributes.statements.Count != 0)
            //    str.Append(")");
            //IndentAndNewline();
            return null;
        }

        public override object Visit(FuncDeclNode node, object o)
        {
            IndentAndNewline();
            str.Append("func " + node.SymbolObject.Name + "(");
            if (node.Parameters.statements.Count > 0)
                foreach (Node n in node.Parameters.statements)
                {
                    n.Accept(this, null);
                    RemoveIndentAndNewline();
                    str.Append(", ");
                }
            if (node.Parameters.statements.Count > 0)
                RemoveIndentAndNewline();
            str.Append(")");
            str.AppendLine();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            foreach (Node n in node.Body.statements)
            {
                n.Accept(this, null);
            }
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(VarDeclNode node, object o)
        {
            str.Append(node.GetVarType() + " " + node.SymbolObject.Name);
            if (node.DefaultValue != null)
            {
                str.Append(" = ");
                if (node.DefaultValue.GetType() == typeof(CollecConst))
                    str.Append("{");
                node.DefaultValue.Accept(this, null);
                if (node.DefaultValue.GetType() == typeof(CollecConst))
                    str.Append("}");
            }
            IndentAndNewline();
            return null;
        }

        public override object Visit(VertexDeclNode node, object o)
        {
            str.Append("vertex(");
            str.Append(node.SymbolObject.Name);
            if (node.Attributes.statements.Count > 0)
                str.Append(", ");
            foreach (Node n in node.Attributes.statements)
            {
                n.Accept(this, null);
                RemoveIndentAndNewline();
                str.Append(", ");
            }
            if (node.Attributes.statements.Count > 0)
                RemoveLastCommaAndSpace();
            str.Append(")");
            IndentAndNewline();
            return null;
        }

        public override object Visit(AssignNode node, object o)
        {
            str.Append("");
            node.Target.Accept(this, null);
            str.Append(" = ");
            node.Value.Accept(this, null);
            IndentAndNewline();
            return null;
        }

        public override object Visit(BlockNode node, object o)
        {
            foreach (Node n in node.statements)
            {
                if(n != null)
                n.Accept(this, null);
            }
            return null;
        }

        public override object Visit(ForeachNode node, object o)
        {
            str.Append("foreach(");
            node.IterationVar.Accept(this,null);
            RemoveIndentAndNewline();
            str.Append(" in ");
            node.Iterator.Accept(this, null);
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this, null);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(ForNode node, object o)
        {
            str.Append("for(");
            node.Initializer.Accept(this, null);
            RemoveIndentAndNewline();
            str.Append(", ");
            node.Condition.Accept(this, null);
            str.Append(", ");
            node.Iterator.Accept(this, null);
            RemoveIndentAndNewline();
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this, null);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(HeadNode node, object o)
        {
            if (str.Length != 0)
                RemoveIndentAndNewline();
            str.Append("[" + node.getName().ToLower() + "(");
            foreach (Node n in node.attrDeclBlock.statements)
            {
                n.Accept(this, null);
                RemoveIndentAndNewline();
                str.Append(", ");
            }
            RemoveLastCommaAndSpace();
            str.Append(")]");
            IndentAndNewline();
            IndentAndNewline();
            return null;
        }

        public override object Visit(IfNode node, object o)
        {
            if (node.Condition != null)
            {
                str.Append("if (");
                node.Condition.Accept(this, null);
                str.Append(")");
            }
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this, null);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("} ");
            if (node.ElseNode != null)
            {
                str.Append("else");
                node.ElseNode.Accept(this, null);
            }
            else IndentAndNewline();
            return null;
        }

        public override object Visit(LoneCallNode node, object o)
        {
            node.Call.Accept(this, null);
            IndentAndNewline();
            return null;
        }

        public override object Visit(ReturnNode node, object o)
        {
            str.Append("return ");
            node.Ret.Accept(this, null);
            IndentAndNewline();
            return null;
        }

        public override object Visit(WhileNode node, object o)
        {
            str.Append("while(");
            node.Condition.Accept(this, null);
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            foreach (Node n in node.Body.statements)
            {
                n.Accept(this, null);
            }
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(BreakNode node, object o)
        {
            str.Append(" break ");
            IndentAndNewline();
            return null;
        }

        public override object Visit(ContinueNode node, object o)
        {
            str.Append(" continue ");
            IndentAndNewline();
            return null;
        }

        public override object Visit(MAGIA node, object o)
        {
            node.block.Accept(this, null);
            return null;
        }

        //Will make a newline and ident to the current indent level.
        public void IndentAndNewline()
        {
            str.AppendLine();
            for (int i = indentLevel*indentSizeInSpaces; i > 0; i--)
                str.Append(" ");
        }

        //First removes all the indents of the current level and then adds a newline
        //Is acutally just a function that revereses whatever IndentAndNewline function does.
        public void RemoveIndentAndNewline() {
            for (int i = indentLevel * indentSizeInSpaces; i > 0; i--)
                str.Remove(str.Length - 1, 1);
            str.Remove(str.Length - 2, 2); //Remove the newline.
        }

        //Function with the purpose of removing Comma and space,
        //It actually just removes the last two added chars in the code.
        public void RemoveLastCommaAndSpace()
        {
            str.Remove(str.Length - 2, 2); //Remove the last comma and space.
        }

        public override object Visit(MultiDecl multiDecl, object p)
        {
            throw new NotImplementedException();
        }
    }
}
