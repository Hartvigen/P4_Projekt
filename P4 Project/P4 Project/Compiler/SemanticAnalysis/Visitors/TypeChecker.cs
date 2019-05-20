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
    public sealed class TypeChecker : Visitor
    {
        public override string AppropriateFileName { get; } = "symbolInfo.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        private SymTable Table { get; }
        private SymTable ActiveScope { get; set; }


        public TypeChecker(SymTable table)
        {
            Table = table;
            ActiveScope = table;
        }


        public override void Visit(CallNode node)
        {
            node.Parameters.Expressions.ForEach(e => e.Accept(this));

            //We check that parameter types match.
            var parameterList = Table.FindParameterListOfFunction(node.Ident);
            bool validFound = false;

            foreach (var parameters in parameterList)
            {
                validFound = true;
                for (var i = parameters.Count - 1; i > 0; i--)
                {
                    if (node.Parameters.Expressions[i].type.name != parameters[i].name)
                    {
                        validFound = false;
                        break;
                    }
                }

                if (!validFound) 
                    continue;
                
                //We found a valid parameter list that it matches so given that parameter list and the function name
                //We find the return type
                node.type = Table.FindReturnTypeOfFunction(node.Ident, parameters);
                break;
            }

            if (!validFound)
            {
                ErrorList.Add($"No valid parameter set found for overloaded function: '{node.Ident}'");
            }
        }

        //finds variable type, and returns the type.
        public override void Visit(VarNode node)
        {
            node.Source?.Accept(this);

            //We assign the type found from the table.
            if(ActiveScope.Find(node.Ident) != null)
                node.type = ActiveScope.Find(node.Ident).Type;

			//If the Source exist we can find the type as from the attribute that matches from the source
			if (node.type == null && node.Source?.type != null) {
				if (Table.IsAttribute(node.Source.type.name, node.Ident)) {
					node.type = Table.GetTypeOfAttribute(node.Source.type.name, node.Ident);
				}
			}

			switch (node.type)
            {
                case null when node.Source != null:
                    ErrorList.Add("No type given var: " + node.Source.Ident + "." + node.Ident);
                    return;
                case null:
                    ErrorList.Add("No type given var: " + node.Ident);
                    break;
                default: return;
            }
        }

        //returns a Bool type
        public override void Visit(BoolConst node)
        {
            if (node.type is null)
                node.type = new BaseType("boolean");
            else if (node.type.name != "boolean")
                ErrorList.Add("BoolConst is always type boolean but was found to be of type: " + node.type.name);
        }

        //Checks if all elements in a collection is the same type, and returns the type.
        public override void Visit(CollecConst node)
        {
            //If the Collection contains no elements it is type correct no matter what.
            if (node.Expressions.Count == 0)
                return;

            //We check that each Expression in the Collection evaluates to the same type as the collection.
            node.Expressions.ForEach(n =>
            {
                n.Accept(this);
                if (node.type.singleType.name != n.type.name) {
                    ErrorList.Add("Collection contains element of type: " + node.Expressions[0].type + " but collection is type " + node.type.singleType.name);
                }
            });
        }

        //returns null
        public override void Visit(NoneConst node)
        {
            if (node.type is null)
                node.type = new BaseType("none");
            else if (node.type.name != "none")
                ErrorList.Add("NoneConst is always type none but was found to be type: " + node.type.name);
        }

        //returns a numberType
        public override void Visit(NumConst node)
        {
            if(node.type is null)
                node.type = new BaseType("number");
            else if (node.type.name != "number")
                    ErrorList.Add("NumConst is always type number but was found to be type: " + node.type.name);
        }

        //returns a text
        public override void Visit(TextConst node)
        {
            if (node.type is null)
                node.type = new BaseType("text");
            if (node.type.name != "text")
                ErrorList.Add("TextConst is always type text but was found to be type: " + node.type.name);
        }

        //Checks that the correct types are used in the BinExprNode
        public override void Visit(BinExprNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            var l = node.Left.type;
            var r = node.Right.type;

            if (l == null || r == null)
                return;

            if (l.name == "func")
                l.name = l.returnType;

            if (r.name == "func")
                r.name = r.returnType;

            if (Operators.GetResultingTypeFromOperandTypeAndOperator(l, node.OperatorType) is null)
                ErrorList.Add("The operator: " + Operators.GetCodeFromInt(node.OperatorType) + " cannot be used with type: " + l);
            else node.type = Operators.GetResultingTypeFromOperandTypeAndOperator(l, node.OperatorType);

            if (l.name != r.name)
                ErrorList.Add("BinExprNode has differentiating operand types: " + l.name +  " and " + node.GetCodeOfOperator() + " " + r.name);
        }
        public override void Visit(UnaExprNode node)
        {
            node.Expr.Accept(this);

            if (Operators.GetResultingTypeFromOperandTypeAndOperator(node.Expr.type, node.OperatorType) is null) {
                ErrorList.Add("The operator: " + Operators.GetCodeFromInt(node.OperatorType) + " cannot be used with type: " + node.Expr.type);
                return;
            }
            node.type = Operators.GetResultingTypeFromOperandTypeAndOperator(node.Expr.type, node.OperatorType);
        }

        //checks if both types of an EdgeCreateNode is an vertex
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

                    //We find the header scope for edge take the type of the attribute
                    Table.GetScopes().ForEach(h =>
                    {
                        if (!h.header || h.name != "edge") return;
                        if (h.Find(a.Target.Ident).Type.name != a.Value.type.name)
                            ErrorList.Add(a.Target.Ident + " is type: " + a.Target.type.name + " so type: " + a.Value.type.name + " is not a valid type to assign.");
                    });
                });
            });

            if (node.LeftSide.type.name != "vertex") {
                ErrorList.Add("Edge cannot be created with: " + node.LeftSide.Ident + " as it has type: " + node.LeftSide.type);
            }
            node.RightSide.ForEach(n => {
                if (n.Item1.type.name != "vertex")
                    ErrorList.Add("Edge cannot be created with: " + n.Item1.Ident + " as it has type: " + n.Item1.type.name);
            });
        }

        public override void Visit(FuncDeclNode node)
        {
            EnterFunction(node.SymbolObject.Name);
            node.Parameters.Accept(this);   
            node.Body.Accept(this);
            
            //if body has a return type we make sure they match the stated return type
            node.Body.Statements.ForEach(stmtNode =>
            {
                if (stmtNode.GetType() != typeof(ReturnNode)) return;
                var retNode = (ReturnNode) stmtNode;

                var actualReturnType = retNode.Ret.type.name;
                var declaredReturnType = node.SymbolObject.Type.returnType;
                if (actualReturnType != declaredReturnType)
                {
                    ErrorList.Add(
                        "There was found a return type in Function: " + node.SymbolObject.Name + 
                        " with declared return type: " + declaredReturnType + " but actual return type was: " + actualReturnType);
                }
            });
            LeaveFunction();
        }

        //Checks for default and SymbolBaseTypeValue. If it doesnt exist, create the SymbolBaseType
        public override void Visit(VarDeclNode node)
        {
            //If there is not specified any default value there is nothing to type check
            if (node.DefaultValue == null)
                return;

            node.DefaultValue.Accept(this);

            if (node.DefaultValue.type == null)
                return;

            if (node.DefaultValue.type.name == "func")
            {
                if (node.DefaultValue.type.returnType != node.SymbolObject.Type.name)
                    ErrorList.Add("Cannot initialize variable " + node.SymbolObject.Name + " with call that returns: " + node.DefaultValue.type.returnType + " when variable is type: " + node.type);
            }else if (node.DefaultValue.type.name != node.type.name && node.DefaultValue.type.name != "none")
                ErrorList.Add("Cannot initialize variable " + node.SymbolObject.Name + " with of type: " + node.DefaultValue.type + " when variable is type: " + node.type);
        }

        //Creates a vertex BaseType in SymbolTable
        public override void Visit(VertexDeclNode node)
        {
            //Vertex cant be typeChecked like normal as their type is not stored in activeScope
            if (node.Attributes.Statements.Count != 0)
            {
                node.Attributes.Statements.ForEach(s => {
                    if (s.GetType() != typeof(AssignNode)) return;
                    var a = (AssignNode)s;
                    a.Value.Accept(this);
                    //We find the header scope for vertex and if the attribute is not there it is invalid.
                    Table.vertexAttr.GetVariables().TryGetValue(a.Target.Ident, out Obj o);
                    if(o != null && o.Type.name != a.Value.type.name)
                        ErrorList.Add(o.Name + " is type " + o.Type.name + " cannot be assigned type: " + a.Value.type.name);
                });
            }
        }

        //Checks for an assigned value is in a correct type. like sum = 0. Which fails if sum is not a number.
        public override void Visit(AssignNode node)
        {
            node.Target.Accept(this);
            node.Value.Accept(this);

            if (ActiveScope.Find(node.Target.Ident) == null) {
                //If there is a source it is not a problem
                if (node.Target.Source != null)
                    return;
                ErrorList.Add(node.Target.Ident + " was not found in the symbolTable.");
                return;
            }

            var t = ActiveScope.Find(node.Target.Ident).Type;
            var v = node.Value.type;

            if (v.name == "func")
            {
                if (v.returnType != t.name)
                {
                    ErrorList.Add("Call returns a type: " + v.returnType + "but needs type: " + t.name);
                    return;
                }
                else
                    return;
            }

            if (t.name != v.name)
            {
                ErrorList.Add("Incompatible types in Assign of variable: " + t.name + " and " + v.name);
            }
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

            if (ActiveScope.Find(node.IterationVar.SymbolObject.Name) is null)
                ActiveScope.NewObj(node.IterationVar.SymbolObject.Name, node.IterationVar.type, node.IterationVar.SymbolObject.Kind);

            if (node.Iterator.type.name != "collec")
                ErrorList.Add("The iterator in a foreach must be a collection!");

            if (node.Iterator.type.singleType.name != node.IterationVar.type.name)
                ErrorList.Add("Foreach loop has collection type: " + node.Iterator.type.singleType.name + " and the variable has type: " + node.IterationVar.type.name);
            LeaveThisScope();
        }

        public override void Visit(ForNode node)
        {
            EnterNextScope();
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            if (node.Condition.type.name != "boolean")
                ErrorList.Add("Condition of for loop must be type boolean");
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            LeaveThisScope();
        }

        public override void Visit(HeadNode node)
        {
            node.attrDeclBlock.Accept(this);
        }

        //Checks if the condition is a bool
        public override void Visit(IfNode node)
        {
            EnterNextScope();
            node.Condition?.Accept(this);

            if (node.Condition?.type != null)
                if (node.Condition.type.name != "boolean" && node.Condition.type.name != "none")
                {
                    if (node.Condition.type.name != "")
                    {
                        ErrorList.Add("If-statements must have boolean type or be none and not " + node.Condition.type.name);
                        return;
                    } 
                }
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

            if (node.Condition.type == null)
                return;

            //The Condition must be type boolean.
            if (node.Condition.type.name != "boolean")
                ErrorList.Add("The condition in a while loop must be boolean type!");

            node.Body.Accept(this);

            LeaveThisScope();
        }
        
        public override void Visit(Magia node)
        {
            //We reset the ScopePositions
            Table.ResetScopePositions();
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
