using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.SymbolTable;

namespace P4_Project.Compiler.SemanticAnalysis.Visitors
{
    public sealed class ScopeChecker : Visitor
    {
        //The Scope visitor ensures that all accessed variables are in their legitimate scopes
        //The Scope Visitor does NOT create the scopes, that is handled as the symbolTable is filled in the parser
        //This visitor does however ensure that:
        //1. Variables have been declared in a legal scope before being accessed
        //2. Only parameters declared in the vertex and edge head can be given when declaring a vertex or edge
        //3. That we are in the correct scope at any given time in a program
        //4. That we cannot assign a variable to a value of a different type (for example setting a number variable x to true should not be possible)
        //5. Variables have been assigned a value before we try to use them

        public override string AppropriateFileName { get; } = "Errors";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public SymTable Table { get; }
        private SymTable ActiveScope { get; set; }


        public ScopeChecker(SymTable table)
        {
            Table = table;
            ActiveScope = table;
        }


        public override void Visit(CallNode node)
        {
            node.Parameters.Accept(this);   
        }

        public override void Visit(VarNode node)
        {
            node.Source?.Accept(this);

            //If the Source (and type of) is not null this must be an attribute of the source type  
            if (node.Source != null)
            {
                if (ActiveScope.Find(node.Source.Ident) == null)
                {
                    ErrorList.Add(node.Source.Ident + " has no declaration in scope.");
                    return;
                }

                if (node.Source.type == null)
                    node.Source.type = ActiveScope.Find(node.Source.Ident).Type;

                if (!Table.IsAttribute(node.Source.type.name, node.Ident))
                    ErrorList.Add(node.Ident + " is not a valid attribute of: " + node.Source.type.name);
                return;
            }

            //If the source (or type of) is null we have to able to find the declaration in the scope and already reached! 
            if (ActiveScope.Find(node.Ident) == null)
                ErrorList.Add($"'{node.Ident}' is not in the scope!");
            else if (!ActiveScope.Find(node.Ident).Type.reached)
                ErrorList.Add($"'{node.Ident}' is in the scope, but not declared before use!");
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
            node.RightSide.ForEach(t => {
                t.Item1.Accept(this);
                t.Item2.ForEach(s => {
                    if (s.GetType() != typeof(AssignNode)) return;
                    var a = s;
                    //We dont care about the right side of the assign it must still be valid according to all scope rules 
                    a.Value.Accept(this);
                    //We find the header scope for edge and if the attribute is not there it is invalid. 
                    Table.GetScopes().ForEach(h =>
                    {
                        if (!h.header || h.name != "edge") return;
                        if (h.Find(a.Target.Ident) == null && !PreDefined.PreDefinedAttributesEdge.Contains(a.Target.Ident))
                            ErrorList.Add(a.Target.Ident + " is not a valid attribute for edge");
                    });
                });
            });
        }

        public override void Visit(FuncDeclNode node)
        {
            EnterFunction(node.SymbolObject.Name);
            node.Parameters.Accept(this);
            node.Body.Accept(this);
            LeaveFunction();
        }

        public override void Visit(VarDeclNode node)
        {
            node.DefaultValue?.Accept(this);

            if (ActiveScope.Find(node.SymbolObject.Name).Type is null)
                ActiveScope.Find(node.SymbolObject.Name).Type = node.type;


            //It is marked that this declaration has been reached! 
            ActiveScope.Find(node.SymbolObject.Name).Type.reached = true;
        }
        public override void Visit(VertexDeclNode node)
        {
            if (node.Attributes.Statements.Count != 0)
            {
                node.Attributes.Statements.ForEach(s => {
                    if (s.GetType() == typeof(AssignNode))
                    {
                        AssignNode a = (AssignNode)s;
                        //We dont care about the right side of the assign it must still be valid according to all scope rules 
                        a.Value.Accept(this);

                        //We find the header scope for vertex and if the attribute is not there it is invalid. 
                        if(!Table.vertexAttr.GetDic().ContainsKey(a.Target.Ident) && !PreDefined.PreDefinedAttributesVertex.Contains(a.Target.Ident))
                            ErrorList.Add(a.Target.Ident + " is not a valid attribute for vertex");
                    }
                });
            }

            //The vertex has not been reached so we set reached to true 
            ActiveScope.Find(node.SymbolObject.Name).Type.reached = true;
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
            EnterNextScope();
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            LeaveThisScope();
        }

        public override void Visit(ForNode node)
        {
            EnterNextScope();
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            LeaveThisScope();
        }

        public override void Visit(HeadNode node)
        {
        }

        public override void Visit(IfNode node)
        {
            EnterNextScope();
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            LeaveThisScope();
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
            EnterNextScope();
            node.Condition.Accept(this);
            node.Body.Accept(this);
            LeaveThisScope();
        }

        public override void Visit(Magia node)
        {
            node.block.Accept(this);
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

        private void LeaveThisScope()
        {
            ActiveScope = ActiveScope.CloseScope();
        }

        private void EnterNextScope()
        {
            ActiveScope = ActiveScope.EnterNextScope();
        }

        private void EnterFunction(string name)
        {
            Table.GetScopes().ForEach(s => {
                if (s.name == name)
                {
                    ActiveScope = s;
                }
            });
        }

        private void LeaveFunction()
        {
            ActiveScope = Table;
        }
    }
}