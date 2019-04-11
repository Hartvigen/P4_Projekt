using System;
using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Types.Functions;

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
            if (node.Source != null)
            {
                node.Source.Accept(this, null);
                str.Append(".");
            }
            str.Append(node.Identifier + "(");
            node.Parameters.Accept(this, null);
            str.Append(")");
            return null;
        }

        public override object Visit(VarNode node, object o)
        {
            if (node.InParentheses)
                str.Append("(");
            if (node.Source != null)
            {
                node.Source.Accept(this, null);
                str.Append(".");
            }

            str.Append(node.Identifier);

            if (node.InParentheses)
                str.Append(")");
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
                CommaAndSpace();
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
            node.LeftSide.Accept(this, null);
            str.Append(" " + node.GetCodeofOperator() + " ");
            if (node.RightSide.Count == 1)
            {
                if (node.RightSide[0].Item2.Count != 0)
                    str.Append("(");

                node.RightSide[0].Item1.Accept(this, null);
                CommaAndSpace();

                node.RightSide[0].Item2.ForEach(l => { l.Accept(this, null); RemoveIndentAndNewline(); CommaAndSpace(); });

                RemoveLastCommaAndSpace();

                if (node.RightSide[0].Item2.Count != 0)
                    str.Append(")");
            }
            else
            {
                str.Append("{");
                node.RightSide.ForEach(t =>
                {
                    str.Append("(");
                    t.Item1.Accept(this, null);
                    CommaAndSpace();

                    t.Item2.ForEach(l => { l.Accept(this, null); RemoveIndentAndNewline(); CommaAndSpace(); });

                    RemoveLastCommaAndSpace();
                    str.Append(")");
                    CommaAndSpace();
                });
                RemoveLastCommaAndSpace();
                str.Append("}");
            }
            IndentAndNewline();
            return null;
        }

        public override object Visit(FuncDeclNode node, object o)
        {
            IndentAndNewline();
            str.Append("func " + (((FunctionType)node.SymbolObject.Type).ReturnType?.ToString() ?? "none") + " " + node.SymbolObject.Name + "(");
            if (node.Parameters.statements.Count != 0)
            {
                foreach (Node n in node.Parameters.statements)
                {
                    n.Accept(this, null);
                    RemoveIndentAndNewline();
                    CommaAndSpace();
                }
                RemoveLastCommaAndSpace();
            }

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
            CommaAndSpace();
            node.Attributes.statements.ForEach(v => {
                v.Accept(this, null);
                RemoveIndentAndNewline();
                CommaAndSpace();
            });
            RemoveLastCommaAndSpace();
            str.Append(")");
            IndentAndNewline();
            return null;
        }

        public override object Visit(AssignNode node, object o)
        {
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
                if (n != null)
                    n.Accept(this, null);
            }
            return null;
        }

        public override object Visit(ForeachNode node, object o)
        {
            str.Append("foreach(");
            node.IterationVar.Accept(this, null);
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
            CommaAndSpace();
            node.Condition.Accept(this, null);
            CommaAndSpace();
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
            str.Append("[" + node.getName() + "(");
            foreach (Node n in node.attrDeclBlock.statements)
            {
                n.Accept(this, null);
                RemoveIndentAndNewline();
                CommaAndSpace();
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

        public override object Visit(MultiDecl node, object o)
        {
            if (node.Decls[0].GetType().Name == "VertexDeclNode")
            {
                if (node.Decls.Count > 1)
                {
                    str.Append("vertex{");
                    node.Decls.ForEach(d =>
                    {
                        str.Append("(");
                        str.Append(d.SymbolObject.Name);
                        CommaAndSpace();
                        ((VertexDeclNode)d).Attributes.statements.ForEach(v => {
                            v.Accept(this, null);
                            RemoveIndentAndNewline();
                            CommaAndSpace();
                        });
                        RemoveLastCommaAndSpace();
                        str.Append(")");
                        CommaAndSpace();
                    });
                    RemoveLastCommaAndSpace();
                    str.Append("}");
                }
                else
                {
                    str.Append("vertex(");
                    node.Decls[0].Accept(this, null);
                    str.Append(")");
                }
            }
            else
            {
                Console.WriteLine(node.Decls.GetType().Name);
                node.Decls.ForEach(n => { n.Accept(this, null); IndentAndNewline(); });
                RemoveIndentAndNewline();
            }
            IndentAndNewline();
            return null;
        }

        //Will make a newline and ident to the current indent level.
        public void IndentAndNewline()
        {
            str.Append(Environment.NewLine);
            for (int i = indentLevel * indentSizeInSpaces; i > 0; i--)
                str.Append(" ");
        }

        //Will make a comma and space.
        public void CommaAndSpace()
        {
            str.Append(", ");
        }

        //First removes all the indents of the current level and then adds a newline
        //Is acutally just a function that revereses whatever IndentAndNewline function does.
        public void RemoveIndentAndNewline()
        {
            str.Length -= indentLevel * indentSizeInSpaces + Environment.NewLine.Length; //Moves the pointer back the appropriate amount.
        }

        //Function with the purpose of removing Comma and space,
        //It actually just moves the pointer two places back.
        public void RemoveLastCommaAndSpace()
        {
            str.Length -= 2; //Moves the pointer 2 back so the chars are "removed/forgotten"
        }
    }
}