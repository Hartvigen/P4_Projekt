using System.Collections.Generic;
using System.Linq;
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

            // We had to do this here, so that something like "function().attribute" can be scope checked
            TypeChecker tempTypeChecker = new TypeChecker(Table);
            node.Parameters.Expressions.ForEach(exp => exp.Accept(tempTypeChecker));
            node.type = Table.FindReturnTypeOfFunction(node.Ident, node.Parameters.Expressions.Select(exp => exp.type).ToList());
        }

        public override void Visit(VarNode node)
        {
            node.Source?.Accept(this);

            //If the Source is not null this must be an attribute of the source type  
            if (node.Source != null)
            {
                // Check if 'node.Ident' is a valid atribute in the type denoded by 'node.Source.type.name'
                if (node.Source.type.name == "func" && Table.IsAttribute(node.Source.type.returnType.name, node.Ident))
                    node.type = Table.GetTypeOfAttribute(node.Source.type.returnType.name, node.Ident);
                else if (Table.IsAttribute(node.Source.type.name, node.Ident))
                    node.type = Table.GetTypeOfAttribute(node.Source.type.name, node.Ident);
                else
                    ErrorList.Add(node.Ident + " is not a valid attribute of: " + node.Source.type.name);
            }
            else
            {
                //If the source is null we have to able to find the declaration in the scope and already reached! 
                Obj obj = ActiveScope.FindVar(node.Ident);

                if (obj == null)
                    ErrorList.Add($"'{node.Ident}' is not in the scope!");
                else
                {
                    if (!ActiveScope.FindVar(node.Ident).Type.reached)
                        ErrorList.Add($"'{node.Ident}' is in the scope, but not declared before use!");

                    // If node does not contain the type of the variable, load that type into the node
                    if (node.type == null)
                        node.type = ActiveScope.FindVar(node.Ident).Type;
                }
            }
        }

        public override void Visit(BoolConstNode node)
        {
            
        }

        public override void Visit(CollecConstNode node)
        {
            
        }

        public override void Visit(NoneConstNode node)
        {
            
        }

        public override void Visit(NumConstNode node)
        {
            
        }

        public override void Visit(TextConstNode node)
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
                    //We dont care about the right side of the assign it must still be valid according to all scope rules 
                    s.Value.Accept(this);
                    //We find the header scope for edge and if the attribute is not there it is invalid. 
                    if (Table.edgeAttr.FindVar(s.Target.Ident) == null && !PreDefined.PreDefinedAttributesEdge.Contains(s.Target.Ident))
                        ErrorList.Add(s.Target.Ident + " is not a valid attribute for edge");
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

            Obj obj = ActiveScope.FindVar(node.SymbolObject.Name);
            if (obj.Type is null)
                obj.Type = node.type;

            //It is marked that this declaration has been reached! 
            obj.Type.reached = true;
        }
        public override void Visit(VertexDeclNode node)
        {
            if (node.Attributes.Statements.Count != 0)
            {
                node.Attributes.Statements.ForEach(s => {                   
                        AssignNode a = (AssignNode)s;

                        //We dont care about the right side of the assign it must still be valid according to all scope rules 
                        a.Value.Accept(this);

                        //We find the header scope for vertex and if the attribute is not there it is invalid. 
                        if(!Table.vertexAttr.GetVariables().ContainsKey(a.Target.Ident) && !PreDefined.PreDefinedAttributesVertex.Contains(a.Target.Ident))
                            ErrorList.Add(a.Target.Ident + " is not a valid attribute for vertex");                    
                });
            }

            //The vertex has not been reached so we set reached to true 
            ActiveScope.FindVar(node.SymbolObject.Name).Type.reached = true;
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
           //Scope Positions are reset so other visitors have a clean position tree no matter what.
           Table.ResetScopePositions();
        }

        public override void Visit(BreakNode node)
        {
            
        }

        public override void Visit(ContinueNode node)
        {
            
        }

        public override void Visit(MultiDeclNode node)
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
            Table.GetInnerScopes().ForEach(s => {
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