using System;
using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.SymbolTable;

namespace P4_Project.Visitors
{
    public class TypeVisitor : Visitor
    {
        public override string AppropriateFileName { get; } = "symbolInfo.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public override SymTable Table { get; set; }

        private const bool Verbose = false;

        public TypeVisitor(SymTable Table)
        {
            this.Table = Table;
            Table.name = "top";
        }

        public override void Visit(CallNode node)
        {
            node.Parameters.Expressions.ForEach(e=>e.Accept(this));

            //We decorate the node with its type from the table
            node.type = Table.findReturnTypeOfFunction(node.Ident);

            //We check that parameter types match.
            List<BaseType> l = Table.findParameterListOfFunction(node.Ident);
            for (int i = l.Count - 1; i > 0; i--) {
                if (node.Parameters.Expressions[i].type.name != l[i].name) {
                    ErrorList.Add("Wrong parameter type for function " + node.Ident + " should be type: " + l[i] + " but was: " + node.Parameters.Expressions[i].type);
                }
            }
            
        }

        //finds variable type, and returns the type.
        public override void Visit(VarNode node)
        {
            node.Source?.Accept(this);

            //We assign the type found from the table.
            if(Table.Find(node.Ident) != null)
            node.type = Table.Find(node.Ident).type;

            if (node.type == null)
                return;
        }

        //returns a Bool type
        public override void Visit(BoolConst node)
        {
            if (node.type is null)
            {
                node.type = new BaseType("boolean");
            }
            else if (node.type.name != "boolean")
                ErrorList.Add("BoolConst is allways type boolean but was found to be type: " + node.type.name);
        }

        //Checks if all elements in a collection is the same type, and returns the type
        public override void Visit(CollecConst node)
        {
            //We check that each Expression in the Collection evulates to the same type as the first one in the collection
            node.Expressions.ForEach(n =>
            {
                n.Accept(this);
                if (node.Expressions[0].type.name != n.type.name) {
                    ErrorList.Add("Collection contains both: " + node.Expressions[0].type + " and " + n.type);
                }
            });
            //We collection its type defualt is list TODO: find the actual collection type
            if(node.Expressions.Count != 0)
                node.type = new BaseType(new BaseType("list"), node.Expressions[0].type);
        }

        //returns null
        public override void Visit(NoneConst node)
        {
            if (node.type is null)
            {
                node.type = new BaseType("none");
            }
            else if (node.type.name != "none")
                ErrorList.Add("NoneConst is allways type none but was found to be type: " + node.type.name);
        }

        //returns a numberType
        public override void Visit(NumConst node)
        {
            if(node.type is null)
            {
                node.type = new BaseType("number");
            }
            else if (node.type.name != "number")
                    ErrorList.Add("NumConst is allways type number but was found to be type: " + node.type.name);
        }

        //returns a text
        public override void Visit(TextConst node)
        {
            if (node.type is null)
            {
                node.type = new BaseType("text");
            }
            if (node.type.name != "text")
                ErrorList.Add("TextConst is allways type text but was found to be type: " + node.type.name);
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
                l.name = l.returntype;

            if (r.name == "func")
                r.name = r.returntype;

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
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });

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
            node.Parameters.Accept(this);   
            node.Body.Accept(this);
            
            //if body has a return type we make sure they match the stated return type
            node.Body.Statements.ForEach(stmtNode =>
            {
                if (stmtNode.GetType() != typeof(ReturnNode)) return;
                var retNode = (ReturnNode) stmtNode;
                var actualReturnType = retNode.Ret.type.name;
                var declaredReturnType = node.SymbolObject.type.returntype;
                if (actualReturnType != declaredReturnType)
                {
                    ErrorList.Add(
                        "There was found a return type in Function: " + node.SymbolObject.Name + 
                        " with declared return type: " + declaredReturnType + " but actual return type was: " + actualReturnType);
                }
            });
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
                if (node.DefaultValue.type.returntype != node.SymbolObject.type.name)
                {
                    ErrorList.Add("Cannot initialize variable " + node.SymbolObject.Name + " with call that returns: " + node.DefaultValue.type.returntype + " when variable is type: " + node.type);
                }
                else return;
            else if (node.DefaultValue.type.name != node.type.name && node.DefaultValue.type.name != "none")
                ErrorList.Add("Cannot initialize variable " + node.SymbolObject.Name + " with of type: " + node.DefaultValue.type + " when variable is type: " + node.type);
        }

        //Creates a vertex BaseType in SymbolTable
        public override void Visit(VertexDeclNode node)
        {
            node.Attributes.Accept(this);
            if(Table.Find(node.SymbolObject.Name) is null)
                Table.NewObj(node.SymbolObject.Name, node.SymbolObject.type, node.SymbolObject.Kind);
        }

        //Checks for an assigned value is in a correct type. like sum = 0. Which fails if sum is not a number.
        public override void Visit(AssignNode node)
        {
            node.Target.Accept(this);
            node.Value.Accept(this);

            if (Table.Find(node.Target.Ident) == null) {
                ErrorList.Add(node.Target.Ident + " was not found in the symboltable.");
                return;
            }

            BaseType t = Table.Find(node.Target.Ident).type;
            BaseType v = node.Value.type;

            if (v.name == "func")
                if (v.returntype != t.name)
                {
                    ErrorList.Add("Call returns a type: " + v.returntype + "but needs type: " + t.name);
                    return;
                }
                else return;
            if (t.name != v.name)
            {
                ErrorList.Add("Incompatible types in Assign of variable: " + t.name + " and " + v.name);
            }
        }

        //visits BlockNode
        public override void Visit(BlockNode node)
        {
            node.Statements.ForEach(n => n.Accept(this));
        }

        //Visits foreachNode (undone)
        public override void Visit(ForeachNode node)
        {
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);

            if (Table.Find(node.IterationVar.SymbolObject.Name) is null)
                Table.NewObj(node.IterationVar.SymbolObject.Name, node.IterationVar.type, node.IterationVar.SymbolObject.Kind);

            if (node.Iterator.type.name != "collec")
                ErrorList.Add("The iterator in a foreach must be a collection!");

            if (node.Iterator.type.singleType.name != node.IterationVar.type.name)
                ErrorList.Add("Foreach loop has collection type: " + node.Iterator.type.singleType.name + " and the variable has type: " + node.IterationVar.type.name);
        }

        //Visits forNode
        public override void Visit(ForNode node)
        {
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
        }

        //Visits HeadNode
        public override void Visit(HeadNode node)
        {
            node.attrDeclBlock.Accept(this);
        }

        //Checks if the condition is a bool
        public override void Visit(IfNode node)
        {
            node.Condition?.Accept(this);

            if (!(node.Condition is null) && node.Condition.type != null)
                if (node.Condition.type.name != "boolean" && node.Condition.type.name != "none")
                {
                    if (node.Condition.type.name != "")
                    {
                        ErrorList.Add("If statements must have boolean type or be none and not " + node.Condition.type.name);
                        return;
                    } 
                }

            node.Body.Accept(this);
            node.ElseNode?.Accept(this);

            //If the ElseNode exist we visit that as well.
            node.ElseNode?.Accept(this);
        }

        //visits LoneCallNode
        public override void Visit(LoneCallNode node)
        {
            node.Call.Accept(this);
        }

        public override void Visit(ReturnNode node)
        {
            node.Ret.Accept(this);
        }

        //visits WhileNode
        public override void Visit(WhileNode node)
        {
            
            node.Condition.Accept(this);

            if (node.Condition.type == null)
                return;

            //The Condition must be type boolean.
            if (node.Condition.type.name != "boolean")
            ErrorList.Add("The condition in a while loop must be boolean type!");

            node.Body.Accept(this);
        }
        
        //visits MagiaNode
        public override void Visit(Magia node)
        {
            //Dispatch starts here!
            node.block.Accept(this);
        }

        public override void Visit(BreakNode node)
        {
        }

        public override void Visit(ContinueNode node)
        {
        }

        //visits MultiDecl
        public override void Visit(MultiDecl multiDecl)
        {
            multiDecl.Decls.ForEach(n => n.Accept(this));
        }
        


        private BaseType GetTypeOfPreDefFunction(string name)
        {
            switch (name)
            {
                case "GetEdge": return new BaseType("edge");
                case "RemoveEdge": return new BaseType("none");
                case "GetEdges": return new BaseType(new BaseType("set"), new BaseType("edge"));
                case "GetVertex": return new BaseType("vertex");
                case "RemoveVertex": return new BaseType("none");
                case "GetVertices": return new BaseType(new BaseType("set"), new BaseType("vertex"));
                case "ClearEdges": return new BaseType("none");
                case "ClearVertices": return new BaseType("none");
                case "ClearAll": return new BaseType("none");
                default: throw new Exception("\"" + name + "\"" + " is not a supported Function!");
            }
        }
    }
}
