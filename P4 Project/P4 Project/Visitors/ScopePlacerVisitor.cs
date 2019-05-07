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
    public class ScopePlacerVisitor : Visitor
    {
        public override string AppropriateFileName { get; } = "ScopeErrors.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public override SymTable Table { get; set; }

        private bool inVertexHead;
        private bool inEdgeHead;
        private bool inFunction;

        public ScopePlacerVisitor(SymTable table)
        {
            Table = table;
        }

        public override void Visit(CallNode node)
        {
            
            node.Parameters.Accept(this);
            
        }
        public override void Visit(VarNode node)
        {
            node.Source?.Accept(this);
        }

        public override void Visit(BoolConst node)
        {
            
        }

        public override void Visit(CollecConst node)
        {
            
        }

        public override void Visit(NoneConst node)
        {
            
        }

        public override void Visit(NumConst node)
        {
            
        }

        public override void Visit(TextConst node)
        {
            
        }

        public override void Visit(BinExprNode node)
        {
            
            node.Left.Accept(this);
            node.Right.Accept(this);
            
        }

        public override void Visit(UnaExprNode node)
        {
            
            node.Expr.Accept(this);
            
        }

        public override void Visit(EdgeCreateNode node)
        {
            
            node.LeftSide.Accept(this);
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });
            
        }

        public override void Visit(FuncDeclNode node)
        {
            inFunction = true;

            //We remove the FuncDecl from top scope as it is in the functionscope
            Table.RemoveObj(node.SymbolObject);

            node.Parameters.Accept(this);
            node.Body.Accept(this);

            inFunction = false;
        }

        public override void Visit(VarDeclNode node)
        {
            node.DefaultValue?.Accept(this);

            //We make sure to move the VarDeclNode from the generel symboltable to a head symboltable if it exists inside a headnode.
            if (inEdgeHead || inVertexHead)
            {
                Table.GetScopes().ForEach(t =>
                {
                    if (t.header)
                    {
                        t.GetScopes().ForEach(h =>
                        {
                            if (h.name == "vertex" && inVertexHead)
                            {
                                Obj obj = Table.Find(node.SymbolObject.Name);
                                h.AddObj(obj);
                                Table.RemoveObj(obj);
                            }
                            else if (h.name == "edge" && inEdgeHead)
                            {
                                Obj obj = Table.Find(node.SymbolObject.Name);
                                h.AddObj(obj);
                                Table.RemoveObj(obj);
                            }
                        });
                    }
                });
            }
            else if (inFunction) {
                //If we are in a function we can reach all variables 
            }
        }

        public override void Visit(VertexDeclNode node)
        {
            
            node.Attributes.Accept(this);
            
        }

        public override void Visit(AssignNode node)
        {
            
            node.Target.Accept(this);
            node.Value.Accept(this);
            
        }

        public override void Visit(BlockNode node)
        {
            
            node.Statements.ForEach(n => n.Accept(this));
            
        }

        public override void Visit(ForeachNode node)
        {
            
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            
        }

        public override void Visit(ForNode node)
        {
            
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            
        }

        public override void Visit(HeadNode node)
        {
            //Finds the head scope and enters this headnode
            SymTable h = new SymTable(null,null, node.type.name);
            Table.GetScopes().ForEach(t => { if (t.header) t.GetScopes().Add(h);});

            //Set bool that we are in a head.
            if (node.type.name == "vertex")
                inVertexHead = true;
            else inEdgeHead = true;

            //Visit every node in the head.
            node.attrDeclBlock.Accept(this);
             
            //Remove the indicator.
            inEdgeHead = false;
            inVertexHead = false;
        }

        public override void Visit(IfNode node)
        {
            
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.ElseNode?.Accept(this);
            
        }

        public override void Visit(LoneCallNode node)
        {
            
            node.Call.Accept(this);
            
        }

        public override void Visit(ReturnNode node)
        {
            
            node.Ret.Accept(this);
            
        }

        public override void Visit(WhileNode node)
        {
            
            node.Condition.Accept(this);
            node.Body.Accept(this);
            
        }

        public override void Visit(Magia node)
        {
            //Create a HeaderScope
            SymTable t = new SymTable(null, null, " ");
            t.header = true;
            Table.GetScopes().Add(t);
            node.block.Accept(this);

            Console.WriteLine("Done");
        }

        public override void Visit(BreakNode node)
        {
            
        }

        public override void Visit(ContinueNode node)
        {
            
        }

        public override void Visit(MultiDecl node)
        {
            node.Decls.ForEach(n => n.Accept(this));
            
        }
    }
}