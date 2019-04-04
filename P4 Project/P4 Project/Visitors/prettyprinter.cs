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
        public override void Visit(CallNode node)
        {
            str.Append(node.Identifier + "(");
            node.Parameters.Accept(this);
            str.Append(")");
        }

        public override void Visit(IdentNode node)
        {
            str.Append(node.Identifier);
            IndentAndNewline();
        }

        public override void Visit(MemberNode node)
        {
            if (node.InParentheses)
                str.Append("(");
            node.Source.Accept(this);
            str.Append(".");
            node.MemberIdent.Accept(this);
            if (node.InParentheses)
                str.Append(")");
        }

        public override void Visit(VarNode node)
        {
            str.Append(node.Identifier);
        }

        public override void Visit(BoolConst node)
        {
            str.Append(node.GetString());
        }

        public override void Visit(CollecConst node)
        {
            if (node.Expressions.Count == 0)
                return;
            foreach (Node n in node.Expressions)
            {
                n.Accept(this);
                str.Append(", ");
            }
            RemoveLastCommaAndSpace();
        }

        public override void Visit(NoneConst node)
        {
            str.Append("none");
        }

        public override void Visit(NumConst node)
        {
            str.Append(node.GetString());
        }

        public override void Visit(TextConst node)
        {
            str.Append(node.Value);
        }

        public override void Visit(BinExprNode node)
        {
            if (node.InParentheses)
                str.Append("(");
            node.Left.Accept(this);
            str.Append(" " + node.GetCodeofOperator() + " ");
            node.Right.Accept(this);
            if (node.InParentheses)
                str.Append(")");
        }

        public override void Visit(UnaExprNode node)
        {
            if (node.InParentheses)
                str.Append("(");
            str.Append(node.GetCodeofOperator());
            node.Expr.Accept(this);
            if (node.InParentheses)
                str.Append("(");
        }

        public override void Visit(EdgeCreateNode node)
        {
            node.Start.Accept(this);
            str.Append(node.GetCodeofOperator());
            if (node.Attributes.statements.Count != 0)
                str.Append("(");
            node.End.Accept(this);
            if (node.Attributes.statements.Count != 0)
                str.Append(", ");
            foreach (Node n in node.Attributes.statements)
            {
                n.Accept(this);
                RemoveIndentAndNewline();
                str.Append(", ");
            }
            if(node.Attributes.statements.Count > 0)
            RemoveLastCommaAndSpace();
            if (node.Attributes.statements.Count != 0)
                str.Append(")");
            IndentAndNewline();
        }

        public override void Visit(FuncDeclNode node)
        {
            IndentAndNewline();
            str.Append("func " + node.symbolName + "(");
            if (node.Parameters.statements.Count > 0)
                foreach (Node n in node.Parameters.statements)
                {
                    n.Accept(this);
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
                n.Accept(this);
            }
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
        }

        public override void Visit(VarDeclNode node)
        {
            str.Append(node.GetVarType() + " " + node.symbolName);
            if (node.DefaultValue != null)
            {
                str.Append(" = ");
                if (node.DefaultValue.GetType() == typeof(CollecConst))
                    str.Append("{");
                node.DefaultValue.Accept(this);
                if (node.DefaultValue.GetType() == typeof(CollecConst))
                    str.Append("}");
            }
            IndentAndNewline();
        }

        public override void Visit(VertexDeclNode node)
        {
            str.Append("vertex(");
            str.Append(node.symbolName);
            if (node.Attributes.statements.Count > 0)
                str.Append(", ");
            foreach (Node n in node.Attributes.statements)
            {
                n.Accept(this);
                RemoveIndentAndNewline();
                str.Append(", ");
            }
            if (node.Attributes.statements.Count > 0)
                RemoveLastCommaAndSpace();
            str.Append(")");
            IndentAndNewline();
        }

        public override void Visit(AssignNode node)
        {
            str.Append("");
            node.Target.Accept(this);
            str.Append(" = ");
            node.Value.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(BlockNode node)
        {
            foreach (Node n in node.statements)
            {
                if(n != null)
                n.Accept(this);
            }
        }

        public override void Visit(ForeachNode node)
        {
            str.Append("foreach(");
            node.IterationVar.Accept(this);
            RemoveIndentAndNewline();
            str.Append(" in ");
            node.Iterator.Accept(this);
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
        }

        public override void Visit(ForNode node)
        {
            str.Append("for(");
            node.Initializer.Accept(this);
            RemoveIndentAndNewline();
            str.Append(", ");
            node.Condition.Accept(this);
            str.Append(", ");
            node.Iterator.Accept(this);
            RemoveIndentAndNewline();
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
        }

        public override void Visit(HeadNode node)
        {
            if (str.Length != 0)
                RemoveIndentAndNewline();
            str.Append("[" + node.getName().ToLower() + "(");
            foreach (Node n in node.attrDeclBlock.statements)
            {
                n.Accept(this);
                RemoveIndentAndNewline();
                str.Append(", ");
            }
            RemoveLastCommaAndSpace();
            str.Append(")]");
            IndentAndNewline();
            IndentAndNewline();
        }

        public override void Visit(IfNode node)
        {
            if (node.Condition != null)
            {
                str.Append("if (");
                node.Condition.Accept(this);
                str.Append(")");
            }
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("} ");
            if (node.ElseNode != null)
            {
                str.Append("else");
                node.ElseNode.Accept(this);
            }
            else IndentAndNewline();
        }

        public override void Visit(LoneCallNode node)
        {
            node.Call.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(ReturnNode node)
        {
            str.Append("return ");
            node.Ret.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(WhileNode node)
        {
            str.Append("while(");
            node.Condition.Accept(this);
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            foreach (Node n in node.Body.statements)
            {
                n.Accept(this);
            }
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
        }

        public override void Visit(BreakNode node)
        {
            str.Append(" break ");
            IndentAndNewline();
        }

        public override void Visit(ContinueNode node)
        {
            str.Append(" continue ");
            IndentAndNewline();
        }

        public override void Visit(MAGIA node)
        {
            node.block.Accept(this);
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
    }
}
