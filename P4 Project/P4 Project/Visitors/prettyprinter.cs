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
    class PrettyPrinter : Visitor
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
            str.Append(node.identifier + "(");
            node.parameters.Accept(this);
            str.Append(")");
        }

        public override void Visit(IdentNode node)
        {
            str.Append(node.identifier);
            IndentAndNewline();
        }

        public override void Visit(MemberNode node)
        {
            if (node.inParentheses)
                str.Append("(");
            node.source.Accept(this);
            str.Append(".");
            node.memberIdent.Accept(this);
            if (node.inParentheses)
                str.Append(")");
        }

        public override void Visit(VarNode node)
        {
            str.Append(node.identifier);
        }

        public override void Visit(BoolConst node)
        {
            str.Append(node.getString());
        }

        public override void Visit(CollecConst node)
        {
            if (node.exprs.Count == 0)
                return;
            foreach (Node n in node.exprs)
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
            str.Append(node.getString());
        }

        public override void Visit(TextConst node)
        {
            str.Append(node.value);
        }

        public override void Visit(BinExprNode node)
        {
            if (node.inParentheses)
                str.Append("(");
            node.left.Accept(this);
            str.Append(" " + node.getCodeofOperator() + " ");
            node.right.Accept(this);
            if (node.inParentheses)
                str.Append(")");
        }

        public override void Visit(UnaExprNode node)
        {
            if (node.inParentheses)
                str.Append("(");
            str.Append(node.getCodeofOperator());
            node.expr.Accept(this);
            if (node.inParentheses)
                str.Append("(");
        }

        public override void Visit(EdgeDeclNode node)
        {
            node.start.Accept(this);
            str.Append(node.getCodeofOperator());
            if (node.attributes.statements.Count != 0)
                str.Append("(");
            node.end.Accept(this);
            if (node.attributes.statements.Count != 0)
                str.Append(", ");
            foreach (Node n in node.attributes.statements)
            {
                n.Accept(this);
                RemoveIndentAndNewline();
                str.Append(", ");
            }
            if(node.attributes.statements.Count > 0)
            RemoveLastCommaAndSpace();
            if (node.attributes.statements.Count != 0)
                str.Append(")");
            IndentAndNewline();
        }

        public override void Visit(FuncDeclNode node)
        {
            IndentAndNewline();
            str.Append("func " + node.symbolName + "(");
            if (node.parameters.statements.Count > 0)
                foreach (Node n in node.parameters.statements)
                {
                    n.Accept(this);
                    RemoveIndentAndNewline();
                    str.Append(", ");
                }
            if (node.parameters.statements.Count > 0)
                RemoveIndentAndNewline();
            str.Append(")");
            str.AppendLine();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            foreach (Node n in node.body.statements)
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
            str.Append(node.getVarType() + " " + node.symbolName);
            if (node.expr != null)
            {
                str.Append(" = ");
                if (node.expr.GetType() == typeof(CollecConst))
                    str.Append("{");
                node.expr.Accept(this);
                if (node.expr.GetType() == typeof(CollecConst))
                    str.Append("}");
            }
            IndentAndNewline();
        }

        public override void Visit(VertexDeclNode node)
        {
            str.Append("vertex(");
            str.Append(node.symbolName);
            if (node.attributes.statements.Count > 0)
                str.Append(", ");
            foreach (Node n in node.attributes.statements)
            {
                n.Accept(this);
                RemoveIndentAndNewline();
                str.Append(", ");
            }
            if (node.attributes.statements.Count > 0)
                RemoveLastCommaAndSpace();
            str.Append(")");
            IndentAndNewline();
        }

        public override void Visit(AssignNode node)
        {
            str.Append("");
            node.target.Accept(this);
            str.Append(" = ");
            node.value.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(Block node)
        {
            foreach (Node n in node.statements)
            {
                n.Accept(this);
            }
        }

        public override void Visit(ForeachNode node)
        {
            str.Append("foreach(");
            node.iterationVar.Accept(this);
            RemoveIndentAndNewline();
            str.Append(" in ");
            node.iterator.Accept(this);
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.body.Accept(this);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("}");
            IndentAndNewline();
        }

        public override void Visit(ForNode node)
        {
            str.Append("for(");
            node.initializer.Accept(this);
            RemoveIndentAndNewline();
            str.Append(", ");
            node.condition.Accept(this);
            str.Append(", ");
            node.iterator.Accept(this);
            RemoveIndentAndNewline();
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.body.Accept(this);
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
            str.Append("[" + node.getName() + "(");
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
            if (node.condition != null)
            {
                str.Append("if (");
                node.condition.Accept(this);
                str.Append(")");
            }
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.body.Accept(this);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            str.Append("} ");
            if (node.elseNode != null)
            {
                str.Append("else");
                node.elseNode.Accept(this);
            }
            else IndentAndNewline();
        }

        public override void Visit(LoneCallNode node)
        {
            node.call.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(ReturnNode node)
        {
            str.Append("return ");
            node.ret.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(WhileNode node)
        {
            str.Append("while(");
            node.condition.Accept(this);
            str.Append(")");
            IndentAndNewline();
            str.Append("{");
            indentLevel++;
            IndentAndNewline();
            foreach (Node n in node.body.statements)
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
