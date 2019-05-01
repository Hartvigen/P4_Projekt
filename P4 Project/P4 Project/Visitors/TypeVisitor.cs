using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Types;
using P4_Project.Types.Collections;
using P4_Project.Types.Primitives;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Graphviz;
using P4_Project.SymbolTable;
using P4_Project.Types.Functions;
using P4_Project.Types.Structures;

namespace P4_Project.Visitors
{
    internal class TypeVisitor : Visitor
    {
        public override string AppropriateFileName { get; } = "symbolInfo.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; }
        private List<string> PreDefFunctions { get; set; }
        private static SymTable Table { get; set; }

        private SymTable currentScope;
        private static Parser Parser { get; set; }
        
        private const bool Verbose = true;

        public TypeVisitor(Parser parser)
        {
            Table = parser.tab;
            Parser = parser;
            Table.name = "top";
            currentScope = Table;
            FillPreDefFunctions();
            ErrorList = new List<string>();
        }

        public override BaseType Visit(CallNode node)
        {
            node.Parameters.Accept(this);
            
            //If it is a predefined Function we can look it up else we return the value from the Table.
            if (PreDefFunctions.Contains(node.Ident))
            {
                return GetTypeOfPreDefFunction(node.Ident);
            }
            if (Table.Find(node.Ident) is null)
            {
                return null;
            }
            return Table.Find(node.Ident).Type;
        }

        //finds variable type, and returns the type.
        public override BaseType Visit(VarNode node)
        {
            node.Source?.Accept(this);
            if (currentScope.Find(node.Ident) is null)
            {
                ErrorList.Add(node.Ident + " should be declared before use!");
                currentScope.PrintAllInCurrentScope();
                throw new Exception("Stop: " + node.Ident + " should be declared before use.");
                return null;
            }

            if (currentScope.Find(node.Ident).Type is null)
            {
                ErrorList.Add(Table.Find(node.Ident) + " Has null Type!");
                return null;
            }
            return currentScope.Find(node.Ident).Type;
        }

        //returns a Bool type
        public override BaseType Visit(BoolConst node)
        {
            return new BooleanType();
        }

        //Checks if all elements in a collection is the same type, and returns the type
        public override BaseType Visit(CollecConst node)
        {
            if (node.Expressions == null || node.Expressions.Count == 0)
            {
                if (node.Expressions == null)
                {
                    ErrorList.Add("Collection was Null?");   
                }
                return null;
            }
            node.Expressions.ForEach(n =>
            {
                n.Accept(this);
                if (node.Expressions[0].type != n.type)
                    ErrorList.Add("Not the same type in collection");
            });
            return node.type;
        }

        //returns null
        public override BaseType Visit(NoneConst node)
        {
            return new NoneType();
        }

        //returns a numberType
        public override BaseType Visit(NumConst node)
        {
            return new NumberType();
        }

        //returns a text
        public override BaseType Visit(TextConst node)
        {
            return new TextType();    
        }

        //Checks that the correct types are used in the BinExprNode
        public override BaseType Visit(BinExprNode node)
        {
            //In any BinExprNode Both Operands must be present         
            if (node.Left is null || node.Right is null)
            {
                ErrorList.Add("BinExprNode has null operands");
                if (Verbose) Console.WriteLine("BinExprNode has null operands");
                return null;
            }

            var type1 = (BaseType) node.Left.Accept(this);
            var type2 = (BaseType) node.Right.Accept(this);

            node.Type = Operators.GetResultingTypeFromOperandTypeAndOperator(type1, node.OperatorType)[0];
            
            if (type1 == type2) return type1;
            ErrorList.Add("BinExprNode has differentiating operands." + " Types format: " + type1 +  " " + node.Left + " " + node.GetCodeOfOperator() + " " + type2 + " " + node.Right);
            return null;
        }
        
        //Checks for (NOT) and (UMIN) notations
        public override BaseType Visit(UnaExprNode node)
        {
            node.Expr.Accept(this);

            switch (node.OperatorType)
            {
                case 15:
                    node.Type = new BooleanType();
                    break;
                case 9:
                    node.Type = new NumberType();
                    break;
                default: 
                    Console.WriteLine("UnaExprNode can only be \"!\" or \"-\" (UnaryMinus)");
                    break;
            }
            return node.Type;
        }

        //checks if both types of an EdgeCreateNode is an vertex
        public override BaseType Visit(EdgeCreateNode node)
        {
            node.LeftSide.Accept(this);
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });

            if (node.LeftSide.type is null)
            {
             ErrorList.Add("Leftside has no type");
             return null;
            }
            if (node.LeftSide.type.ToString() == "vertex")
            {
                if (node.RightSide.Capacity > 0)
                {
                    node.RightSide.ForEach(n =>
                    {
                        if (Table.Find(n.Item1.Ident).Type != node.LeftSide.type)
                        {
                            ErrorList.Add("There is a type involved that is not a vertex.");    
                        }
                    });
                }
                else
                    ErrorList.Add("The right side was empty.");
            }
            else
                ErrorList.Add("The Left side is not a vertex.");

            if (node.Operator != 16 && node.Operator != 17 && node.Operator != 18)
                ErrorList.Add("EdgeCreateNode has wrong operator type! Must be either -> || -- || <- .");
            
            return null;
        }

        //unfinished try on checking function return
        public override BaseType Visit(FuncDeclNode node)
        {
            EnterScope(node.SymbolObject.Name);

            node.Parameters.Accept(this);   
            node.Body.Accept(this);
            
            //We take the declared return type from the FuncDeclNode
            node.returnType = (((FunctionType) node.SymbolObject.Type).ReturnType?.ToString() ?? "none");
            
            //Foreach Statement in the Body if it is a return type we make sure they match.
            node.Body.Statements.ForEach(stmtNode =>
            {
                if (stmtNode.GetType() != typeof(ReturnNode)) return;
                var ret = (ReturnNode) stmtNode;
                var type1 = ret.Ret.Accept(this) is null ? "none" : ret.Ret.Accept(this).ToString();
                var type2 = node.returnType.ToString();
                if (type1 != type2)
                {
                    ErrorList.Add(
                        "There was found a bad return type in Function: " + node.SymbolObject.Name + 
                        " return should be type: " + node.returnType + " but was: " + ret.Ret.Accept(this));
                }
                if (Verbose) Console.WriteLine("Found return in func: " + node.SymbolObject.Name + " with type: " + type1 + " and expected: " + type2);
            });
            LeaveScope();
            return null;

        }

        //Checks for default and SymbolBaseTypeValue. If it doesnt exist, create the SymbolBaseType
        public override BaseType Visit(VarDeclNode node)
        {
            node.DefaultValue?.Accept(this);
            
            //If the node has a default value then
            if (node.DefaultValue != null)
                return (BaseType)node.DefaultValue.Accept(this);

            if (Table.Find(node.SymbolObject.Name) != null)
            {
                ErrorList.Add( node.SymbolObject.Name + " was previously declared.");
                return null;
            }
            //If there was no problems in the VarDeclNode it is added to the Table.
            Table.NewObj(node.SymbolObject.Name, node.type, node.SymbolObject.Kind);
            return null;
        }

        //Creates a vertex BaseType in SymbolTable
        public override BaseType Visit(VertexDeclNode node)
        {
            node.Attributes.Accept(this);
            Table.NewObj(node.SymbolObject.Name, node.SymbolObject.Type, node.SymbolObject.Kind);
            return null;
        }

        //Checks for an assigned value is in a correct type. like sum = 0. Which fails if sum is not a number.
        public override BaseType Visit(AssignNode node)
        {
            node.Target.Accept(this);
            node.Value.Accept(this);
            if (node.Value.Accept(this) == null)
            {
                ErrorList.Add("Value is missing in Assign");
                return null;
            }
            if(node.Target.Accept(this) == null){
                ErrorList.Add("Target is missing in Assign");
                return null;
            }

            if (Table.Find(node.Target.Ident) == null) return null;
            if (Table.Find(node.Target.Ident).Type != node.Value.type)
            {
                ErrorList.Add("Incompatible types in Assign");
            }
            return null;
        }

        //visits BlockNode
        public override BaseType Visit(BlockNode node)
        {
            node.Statements.ForEach(n => n.Accept(this));
            return null;
        }

        //Visits foreachNode (undone)
        public override BaseType Visit(ForeachNode node)
        {
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            return null;
        }

        //Visits forNode
        public override BaseType Visit(ForNode node)
        {
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            return null;
        }

        //Visits HeadNode
        public override BaseType Visit(HeadNode node)
        { 
            node.attrDeclBlock.Accept(this);
            return null;
        }

        //Checks if the condition is a bool
        public override BaseType Visit(IfNode node)
        {
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.ElseNode?.Accept(this);

            if (!(node.Condition is null))
            if (node.Condition.type != new BooleanType())
            {
                if (node.Condition.type.ToString() !=  "")
                {
                    ErrorList.Add( "If statements must have boolean type or be null and not " + node.Condition.Accept(this));
                    if (Verbose)
                    {
                        Console.WriteLine("If statements must have boolean type or be null and not " +
                                          node.Condition.Accept(this));
                    }
                    return null;
                }
                else
                {
                    if (Verbose)
                    {
                        Console.WriteLine("Ending else node Found");
                    }
                }
            }

            //If the ElseNode exist we visit that as well.
            node.ElseNode?.Accept(this);

            return null;
        }

        //visits LoneCallNode
        public override BaseType Visit(LoneCallNode node)
        {  
            node.Call.Accept(this);
            return null;
        }

        public override BaseType Visit(ReturnNode node)
        {
            node.Ret.Accept(this);
            return node.Ret.type;
        }

        //visits WhileNode (undone)
        public override BaseType Visit(WhileNode node)
        {
            
            node.Condition.Accept(this);
            node.Body.Accept(this);

            //The Condition must be type boolean.
            if (node.Condition.type != new BooleanType())
            ErrorList.Add("The condition in a while loop must be boolean type!");
            
            return null;

        }
        
        //visits MagiaNode
        public override BaseType Visit(Magia node)
        {
            node.block.Accept(this);
            return null;
        }

        public override BaseType Visit(BreakNode node)
        {
            return null;
        }

        public override BaseType Visit(ContinueNode node)
        {
            return null;
        }

        //visits MultiDecl
        public override BaseType Visit(MultiDecl multiDecl)
        {
            multiDecl.Decls.ForEach(n => n.Accept(this));
            return null;
        }

        //prints Table (Used for burglarising, unused at the moment)
        public static void Print()
        {
            var output = new StringBuilder();

            foreach (var keyValuePair in Table.GetDic())
            {
                output.Append($"Name: {keyValuePair.Key} Type: {keyValuePair.Value.Type}");
                output.Append("\n");
            }
            output.Append("\n\n\n");

            foreach (var table in Table.GetScopes())
            {

                foreach (var keyValuePair in table.GetDic())
                {
                    output.Append($"Name: {keyValuePair.Key} Type: {keyValuePair.Value.Type}");
                    output.Append("\n");
                }
                output.Append("\n\n\n");
            }
            Console.WriteLine(output.ToString());
        }
        
        private void FillPreDefFunctions()
        {
            PreDefFunctions = new List<string>
            {
                "Edge",
                "vertices",
                "clear",
                "removeEdge",
                "ClearEdges",
                "ClearVertices",
                "ClearAll"
            };
        }

        private BaseType GetTypeOfPreDefFunction(string func)
        {
            if (!PreDefFunctions.Contains(func))
            {
                throw new NotSupportedException("The function name: " + func + " is not a predefined Function!");
            }

            switch (func)
            {
                case "Edge": return new EdgeType();
                case "vertices": return new ListType(new VertexType());
                default: return null;
            }
        }

        private void EnterScope(string name)
        {
            Table.GetScopes().ForEach(s =>
            {
                if (s.name != name) return;
                if (Verbose)
                    Console.WriteLine("Entering " + name + " scope.");
                currentScope = s;

            });
        }
        
        private void LeaveScope()
        {
            while (currentScope.CloseScope() != null)
            {
                currentScope = currentScope.CloseScope();
            }
        }
    }
}
