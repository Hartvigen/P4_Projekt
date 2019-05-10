using System;
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
    public class PrettyPrinter : Visitor
    {
        public override string AppropriateFileName { get; } = "PrettyCode.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public override SymTable Table { get; set; }

        public PrettyPrinter(SymTable Table)
        {
            this.Table = Table;
        }
        //The level of ident is changed as the visitor moves along so this should start at 0.
        //Changing this will not change the relative indentations,
        //but can be used to change the overall indentation level.
        //Should never be negative.
        private int _indentLevel = 0;

        //The Desired indent size can be changed here 4 is default.
        private const int IndentSizeInSpaces = 4;


        //Below is function that handles each kind of node and prints it pretty complete with
        //spaces, indentation, brackets, spaces and so on...
        public override void Visit(CallNode node)
        {
            if (node.Source != null)
            {
                node.Source.Accept(this);
                Result.Append(".");
            }
            Result.Append(node.Ident + "(");
            node.Parameters.Accept(this);
            Result.Append(")");
        }

        public override void Visit(VarNode node)
        {
            if (node.InParentheses)
                Result.Append("(");
            if (node.Source != null)
            {
                node.Source.Accept(this);
                Result.Append(".");
            }

            Result.Append(node.Ident);

            if (node.InParentheses)
                Result.Append(")");
        }

        public override void Visit(BoolConst node)
        {
            Result.Append(node.GetString());
        }

        public override void Visit(CollecConst node)
        {
            if (node.Expressions.Count == 0)
                return;
            
            node.Expressions.ForEach(n =>
            {
                n.Accept(this);
                CommaAndSpace();
            });
            RemoveLastCommaAndSpace();
        }

        public override void Visit(NoneConst node)
        {
            Result.Append("none");
        }

        public override void Visit(NumConst node)
        {
            Result.Append(node.GetString());
        }

        public override void Visit(TextConst node)
        {
            Result.Append(node.Value);
        }

        public override void Visit(BinExprNode node)
        {
            if (node.InParentheses)
                Result.Append("(");
            node.Left.Accept(this);
            Result.Append(" " + node.GetCodeOfOperator() + " ");
            node.Right.Accept(this);
            if (node.InParentheses)
                Result.Append(")");
        }

        public override void Visit(UnaExprNode node)
        {
            if (node.InParentheses)
                Result.Append("(");
            Result.Append(node.GetCodeOfOperator());
            node.Expr.Accept(this);
            if (node.InParentheses)
                Result.Append("(");
        }

        public override void Visit(EdgeCreateNode node)
        {
            node.LeftSide.Accept(this);
            Result.Append(" " + node.GetCodeOfOperator() + " ");
            if (node.RightSide.Count == 1)
            {
                if (node.RightSide[0].Item2.Count != 0)
                    Result.Append("(");

                node.RightSide[0].Item1.Accept(this);
                CommaAndSpace();

                node.RightSide[0].Item2.ForEach(l => { 
                    l.Accept(this); 
                    RemoveIndentAndNewline(); 
                    CommaAndSpace(); 
                });

                RemoveLastCommaAndSpace();

                if (node.RightSide[0].Item2.Count != 0)
                    Result.Append(")");
            }
            else
            {
                Result.Append("{");
                node.RightSide.ForEach(t =>
                {
                    Result.Append("(");
                    t.Item1.Accept(this);
                    CommaAndSpace();

                    t.Item2.ForEach(l =>
                    {
                        l.Accept(this); 
                        RemoveIndentAndNewline(); 
                        CommaAndSpace();
                    });

                    RemoveLastCommaAndSpace();
                    Result.Append(")");
                    CommaAndSpace();
                });
                RemoveLastCommaAndSpace();
                Result.Append("}");
            }
            IndentAndNewline();
        }

        public override void Visit(FuncDeclNode node)
        {
            IndentAndNewline();
            Result.Append("func " + node.SymbolObject.type.returntype + " ");
            Result.Append(node.SymbolObject.Name + "(");
            if (node.Parameters.Statements.Count != 0)
            {
                foreach (var n in node.Parameters.Statements)
                {
                    n.Accept(this);
                    RemoveIndentAndNewline();
                    CommaAndSpace();
                }
                RemoveLastCommaAndSpace();
            }

            Result.Append(")");
            IndentAndNewline();
            Result.Append("{");
            _indentLevel++;
            IndentAndNewline();
            foreach (var n in node.Body.Statements)
            {
                n.Accept(this);
            }
            RemoveIndentAndNewline();
            _indentLevel--;
            IndentAndNewline();
            Result.Append("}");
            IndentAndNewline();
        }

        public override void Visit(VarDeclNode node)
        {
            if (node.type.name != "collec")
                Result.Append(node.type.name);
            else {
                Result.Append(node.type.collectionType + "<" + node.type.singleType + ">");
            }
            Result.Append(" " + node.SymbolObject.Name);
            if (node.DefaultValue != null)
            {
                Result.Append(" = ");

                if (node.DefaultValue.type.name == "collec" && node.DefaultValue.GetType() == typeof(CollecConst))
                    Result.Append("{");
                node.DefaultValue.Accept(this);
                if (node.DefaultValue.type.name == "collec" && node.DefaultValue.GetType() == typeof(CollecConst))
                    Result.Append("}");
            }
            IndentAndNewline();
        }

        public override void Visit(VertexDeclNode node)
        {
            Result.Append("vertex(");
            Result.Append(node.SymbolObject.Name);
            CommaAndSpace();
            node.Attributes.Statements.ForEach(v => {
                v.Accept(this);
                RemoveIndentAndNewline();
                CommaAndSpace();
            });
            RemoveLastCommaAndSpace();
            Result.Append(")");
            IndentAndNewline();
        }

        public override void Visit(AssignNode node)
        {
            node.Target.Accept(this);
            Result.Append(" = ");
            node.Value.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(BlockNode node)
        {
            foreach (var n in node.Statements)
            {
                n?.Accept(this);
            }
        }

        public override void Visit(ForeachNode node)
        {
            Result.Append("foreach(");
            node.IterationVar.Accept(this);
            RemoveIndentAndNewline();
            Result.Append(" in ");
            node.Iterator.Accept(this);
            Result.Append(")");
            IndentAndNewline();
            Result.Append("{");
            _indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this);
            RemoveIndentAndNewline();
            _indentLevel--;
            IndentAndNewline();
            Result.Append("}");
            IndentAndNewline();
        }

        public override void Visit(ForNode node)
        {
            Result.Append("for(");
            node.Initializer.Accept(this);
            RemoveIndentAndNewline();
            CommaAndSpace();
            node.Condition.Accept(this);
            CommaAndSpace();
            node.Iterator.Accept(this);
            RemoveIndentAndNewline();
            Result.Append(")");
            IndentAndNewline();
            Result.Append("{");
            _indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this);
            RemoveIndentAndNewline();
            _indentLevel--;
            IndentAndNewline();
            Result.Append("}");
            IndentAndNewline();
        }

        public override void Visit(HeadNode node)
        {
            if (Result.Length != 0)
                RemoveIndentAndNewline();
            Result.Append("[" + node.type.name + "(");
            foreach (var n in node.attrDeclBlock.Statements)
            {
                n.Accept(this);
                RemoveIndentAndNewline();
                CommaAndSpace();
            }
            RemoveLastCommaAndSpace();
            Result.Append(")]");
            IndentAndNewline();
            IndentAndNewline();
        }

        public override void Visit(IfNode node)
        {
            if (node.Condition != null)
            {
                Result.Append("if (");
                node.Condition.Accept(this);
                Result.Append(")");
            }
            IndentAndNewline();
            Result.Append("{");
            _indentLevel++;
            IndentAndNewline();
            node.Body.Accept(this);
            RemoveIndentAndNewline();
            _indentLevel--;
            IndentAndNewline();
            Result.Append("} ");
            if (node.ElseNode != null)
            {
                Result.Append("else");
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
            Result.Append("return ");
            node.Ret.Accept(this);
            IndentAndNewline();
        }

        public override void Visit(WhileNode node)
        {
            Result.Append("while(");
            node.Condition.Accept(this);
            Result.Append(")");
            IndentAndNewline();
            Result.Append("{");
            _indentLevel++;
            IndentAndNewline();
            foreach (var n in node.Body.Statements)
            {
                n.Accept(this);
            }
            RemoveIndentAndNewline();
            _indentLevel--;
            IndentAndNewline();
            Result.Append("}");
            IndentAndNewline();
        }

        public override void Visit(BreakNode node)
        {
            Result.Append(" break ");
            IndentAndNewline();
        }

        public override void Visit(ContinueNode node)
        {
            Result.Append(" continue ");
            IndentAndNewline();
        }

        public override void Visit(Magia node)
        {
            node.block.Accept(this);
        }

        public override void Visit(MultiDecl node)
        {
            if (node.Decls[0].SymbolObject.type.name == "vertex")
            {
                if (node.Decls.Count > 1)
                {
                    Result.Append("vertex{");
                    node.Decls.ForEach(d =>
                    {
                        Result.Append("(");
                        Result.Append(d.SymbolObject.Name);
                        CommaAndSpace();
                        ((VertexDeclNode)d).Attributes.Statements.ForEach(v => {
                            v.Accept(this);
                            RemoveIndentAndNewline();
                            CommaAndSpace();
                        });
                        RemoveLastCommaAndSpace();
                        Result.Append(")");
                        CommaAndSpace();
                    });
                    RemoveLastCommaAndSpace();
                    Result.Append("}");
                }
                else
                {
                    Result.Append("vertex(");
                    node.Decls[0].Accept(this);
                    Result.Append(")");
                }
            }
            else
            {
                //Console.WriteLine(node.Decls.GetType().Name);
                node.Decls.ForEach(n => { n.Accept(this); IndentAndNewline(); });
                RemoveIndentAndNewline();
            }
            IndentAndNewline();
        }

        //Will make a newline and ident to the current indent level.
        private void IndentAndNewline()
        {
            Result.Append(Environment.NewLine);
            for (var i = _indentLevel * IndentSizeInSpaces; i > 0; i--)
                Result.Append(" ");
        }

        //Will make a comma and space.
        private void CommaAndSpace()
        {
            Result.Append(", ");
        }

        //First removes all the indents of the current level and then adds a newline
        //Is actually just a function that reverses whatever IndentAndNewline function does.
        private void RemoveIndentAndNewline()
        {
            //Moves the pointer back the appropriate amount.
            Result.Length -= _indentLevel * IndentSizeInSpaces + Environment.NewLine.Length; 
        }

        //Function with the purpose of removing Comma and space,
        //It actually just moves the pointer two places back.
        private void RemoveLastCommaAndSpace()
        {
            Result.Length -= 2; //Moves the pointer 2 back so the chars are "removed/forgotten"
        }
    }
}