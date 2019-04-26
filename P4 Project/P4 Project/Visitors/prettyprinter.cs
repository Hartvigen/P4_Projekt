using System;
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
        public new string appropriateFileName = "prettyprint.txt";
        public new StringBuilder result = new StringBuilder();
        public new int errorCount = 0;

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
                result.Append(".");
            }
            result.Append(node.Identifier + "(");
            node.Parameters.Accept(this, null);
            result.Append(")");
            return null;
        }

        public override object Visit(VarNode node, object o)
        {
            if (node.InParentheses)
                result.Append("(");
            if (node.Source != null)
            {
                node.Source.Accept(this, null);
                result.Append(".");
            }

            result.Append(node.Identifier);

            if (node.InParentheses)
                result.Append(")");
            return null;
        }

        public override object Visit(BoolConst node, object o)
        {
            result.Append(node.GetString());
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
            result.Append("none");
            return null;
        }

        public override object Visit(NumConst node, object o)
        {
            result.Append(node.GetString());
            return null;
        }

        public override object Visit(TextConst node, object o)
        {
            result.Append(node.Value);
            return null;
        }

        public override object Visit(BinExprNode node, object o)
        {
            if (node.InParentheses)
                result.Append("(");
            node.Left.Accept(this, null);
            result.Append(" " + node.GetCodeofOperator() + " ");
            node.Right.Accept(this, null);
            if (node.InParentheses)
                result.Append(")");
            return null;
        }

        public override object Visit(UnaExprNode node, object o)
        {
            if (node.InParentheses)
                result.Append("(");
            result.Append(node.GetCodeofOperator());
            node.Expr.Accept(this, null);
            if (node.InParentheses)
                result.Append("(");
            return null;
        }

        public override object Visit(EdgeCreateNode node, object o)
        {
            node.LeftSide.Accept(this, null);
            result.Append(" " + node.GetCodeofOperator() + " ");
            if (node.RightSide.Count == 1)
            {
                if (node.RightSide[0].Item2.Count != 0)
                    result.Append("(");

                node.RightSide[0].Item1.Accept(this, null);
                CommaAndSpace();

                node.RightSide[0].Item2.ForEach(l => { l.Accept(this, null); RemoveIndentAndNewline(); CommaAndSpace(); });

                RemoveLastCommaAndSpace();

                if (node.RightSide[0].Item2.Count != 0)
                    result.Append(")");
            }
            else
            {
                result.Append("{");
                node.RightSide.ForEach(t =>
                {
                    result.Append("(");
                    t.Item1.Accept(this, null);
                    CommaAndSpace();

                    t.Item2.ForEach(l => { l.Accept(this, null); RemoveIndentAndNewline(); CommaAndSpace(); });

                    RemoveLastCommaAndSpace();
                    result.Append(")");
                    CommaAndSpace();
                });
                RemoveLastCommaAndSpace();
                result.Append("}");
            }
            IndentAndNewline();
            return null;
        }

        public override object Visit(FuncDeclNode node, object o)
        {
            IndentAndNewline();
            result.Append("func " + (((FunctionType)node.SymbolObject.Type).ReturnType?.ToString() ?? "none") + " " + node.SymbolObject.Name + "(");
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

            result.Append(")");
            IndentAndNewline();
            result.Append("{");
            indentLevel++;
            IndentAndNewline();
            foreach (Node n in node.Body.statements)
            {
                n.Accept(this, null);
            }
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            result.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(VarDeclNode node, object o)
        {
            result.Append(node.GetVarType() + " " + node.SymbolObject.Name);
            if (node.DefaultValue != null)
            {
                result.Append(" = ");
                if (node.DefaultValue.GetType() == typeof(CollecConst))
                    result.Append("{");
                node.DefaultValue.Accept(this, null);
                if (node.DefaultValue.GetType() == typeof(CollecConst))
                    result.Append("}");
            }
            IndentAndNewline();
            return null;
        }

        public override object Visit(VertexDeclNode node, object o)
        {
            result.Append("vertex(");
            result.Append(node.SymbolObject.Name);
            CommaAndSpace();
            node.Attributes.statements.ForEach(v => {
                v.Accept(this, null);
                RemoveIndentAndNewline();
                CommaAndSpace();
            });
            RemoveLastCommaAndSpace();
            result.Append(")");
            IndentAndNewline();
            return null;
        }

        public override object Visit(AssignNode node, object o)
        {
            node.Target.Accept(this, null);
            result.Append(" = ");
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
            result.Append("foreach(");
            node.IterationVar.Accept(this, null);
            RemoveIndentAndNewline();
            result.Append(" in ");
            node.Iterator.Accept(this, null);
            result.Append(")");
            IndentAndNewline();
            result.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this, null);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            result.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(ForNode node, object o)
        {
            result.Append("for(");
            node.Initializer.Accept(this, null);
            RemoveIndentAndNewline();
            CommaAndSpace();
            node.Condition.Accept(this, null);
            CommaAndSpace();
            node.Iterator.Accept(this, null);
            RemoveIndentAndNewline();
            result.Append(")");
            IndentAndNewline();
            result.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this, null);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            result.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(HeadNode node, object o)
        {
            if (result.Length != 0)
                RemoveIndentAndNewline();
            result.Append("[" + node.getName() + "(");
            foreach (Node n in node.attrDeclBlock.statements)
            {
                n.Accept(this, null);
                RemoveIndentAndNewline();
                CommaAndSpace();
            }
            RemoveLastCommaAndSpace();
            result.Append(")]");
            IndentAndNewline();
            IndentAndNewline();
            return null;
        }

        public override object Visit(IfNode node, object o)
        {
            if (node.Condition != null)
            {
                result.Append("if (");
                node.Condition.Accept(this, null);
                result.Append(")");
            }
            IndentAndNewline();
            result.Append("{");
            indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this, null);
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            result.Append("} ");
            if (node.ElseNode != null)
            {
                result.Append("else");
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
            result.Append("return ");
            node.Ret.Accept(this, null);
            IndentAndNewline();
            return null;
        }

        public override object Visit(WhileNode node, object o)
        {
            result.Append("while(");
            node.Condition.Accept(this, null);
            result.Append(")");
            IndentAndNewline();
            result.Append("{");
            indentLevel++;
            IndentAndNewline();
            foreach (Node n in node.Body.statements)
            {
                n.Accept(this, null);
            }
            RemoveIndentAndNewline();
            indentLevel--;
            IndentAndNewline();
            result.Append("}");
            IndentAndNewline();
            return null;
        }

        public override object Visit(BreakNode node, object o)
        {
            result.Append(" break ");
            IndentAndNewline();
            return null;
        }

        public override object Visit(ContinueNode node, object o)
        {
            result.Append(" continue ");
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
                    result.Append("vertex{");
                    node.Decls.ForEach(d =>
                    {
                        result.Append("(");
                        result.Append(d.SymbolObject.Name);
                        CommaAndSpace();
                        ((VertexDeclNode)d).Attributes.statements.ForEach(v => {
                            v.Accept(this, null);
                            RemoveIndentAndNewline();
                            CommaAndSpace();
                        });
                        RemoveLastCommaAndSpace();
                        result.Append(")");
                        CommaAndSpace();
                    });
                    RemoveLastCommaAndSpace();
                    result.Append("}");
                }
                else
                {
                    result.Append("vertex(");
                    node.Decls[0].Accept(this, null);
                    result.Append(")");
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
            result.Append(Environment.NewLine);
            for (int i = indentLevel * indentSizeInSpaces; i > 0; i--)
                result.Append(" ");
        }

        //Will make a comma and space.
        public void CommaAndSpace()
        {
            result.Append(", ");
        }

        //First removes all the indents of the current level and then adds a newline
        //Is acutally just a function that revereses whatever IndentAndNewline function does.
        public void RemoveIndentAndNewline()
        {
            result.Length -= indentLevel * indentSizeInSpaces + Environment.NewLine.Length; //Moves the pointer back the appropriate amount.
        }

        //Function with the purpose of removing Comma and space,
        //It actually just moves the pointer two places back.
        public void RemoveLastCommaAndSpace()
        {
            result.Length -= 2; //Moves the pointer 2 back so the chars are "removed/forgotten"
        }
    }
}