using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.SymTab;
using P4_Project.Types;
using P4_Project.Types.Collections;
using P4_Project.Types.Primitives;
using static P4_Project.TypeS;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Types.Functions;
using P4_Project.Types.Structures;

namespace P4_Project.Visitors
{
    class TypeVisitor : Visitor
    {
        public new string appropriateFileName = "typeCheckResult.txt";
        public new StringBuilder result = new StringBuilder();
        public new int errorCount = 0;

        public SymbolTable symbolTable;
        public Parser parser;

        //can be used for function return type
        public BaseType functionReturnType = null;

        public TypeVisitor(Parser parserObject)
        {
            symbolTable = parserObject.tab;
            parser = parserObject;
        }

        //calls function and assign it a type
        public override object Visit(CallNode node, object o)
        {

            BaseType type = null;
            FunctionType returnType;

            //makes sure to not try and find methods in symbolobjects
            if (node.Identifier != "Edge" && node.Identifier != "vertices" && node.Identifier != "clear" && node.Identifier != "removeEdge" && node.Identifier != "ClearEdges" && node.Identifier != "ClearVertices" && node.Identifier != "ClearAll") {

                if (symbolTable.Find(node.Identifier) != null)
                {
                    returnType = (FunctionType)symbolTable.Find(node.Identifier).Type;
                    type = returnType.returnType();
                }
                else
                {
                    type = null;
                } 

                
            }
            if(node.Identifier == "vertices")
            {
                type = new SetType(new VertexType());
            }

            node.Parameters.Accept(this,null);
            return type;
        }

        //finds variable type, and returns the type.
        public override object Visit(VarNode node,object o)
        {
            if (symbolTable.Find(node.Identifier) != null)
                node.type = symbolTable.Find(node.Identifier).Type;
            else return null;
            
            return node.type;
        }

        //returns a Bool type
        public override object Visit(BoolConst node, object o) {return node.type = new BooleanType();}

        //Checks if all elements in a collection is the same type, and returns the type
        public override object Visit(CollecConst node, object o)
        {
            int i = 0;
            foreach (ExprNode n in node.Expressions)
            {
                n.Accept(this, null);
                if (i == 0)
                    node.type = n.type;
                if (node.type != n.type)
                    parser.SemErr("Not the same type in collection");
                i++;
            }
            if (node.Expressions.Capacity == 0)
                return null;
            return node.type;
        }

        //returns null
        public override object Visit(NoneConst node, object o) { return null;}

        //returns a numberType
        public override object Visit(NumConst node, object o)
        {
            if (node != null)
                return node.type = new NumberType();
            else
            {
                parser.SemErr("NumConst is not a number");
                return null;
            }
        }

        //returns a texttype
        public override object Visit(TextConst node, object o)
        {
            if (node != null)
                return node.type = new TextType();
            else
            {
                parser.SemErr("TextConst is not a text");
                return null;
            }
        }

        //Chekcs if the binExprNode should return a boolean or numbertype
        public override object Visit(BinExprNode node, object o)
        {
 
            node.Left.Accept(this, null);
            node.Right.Accept(this, null);

            if (node.Left.Accept(this, null) != null && node.Right.Accept(this, null) != null)
            {


                if ((BaseType)node.Left.Accept(this, null) != (BaseType)node.Right.Accept(this, null)) { 
                    parser.SemErr("BinExprNode is not matching");
                }
            }
            else
                parser.SemErr("BinExprNode is null");

            if (node.OperatorType == 9 || node.OperatorType == 10 || node.OperatorType == 11 || node.OperatorType == 12 || node.OperatorType == 13 || node.OperatorType == 14)
            {
                node.type = new NumberType();
            }
            else
            {
                node.type = new BooleanType();
            }

            return node.type;
        }
        
        //Checks for (NOT) and (UMIN) notations
        public override object Visit(UnaExprNode node, object o)
        {
            node.Expr.Accept(this, null);

            if (node.OperatorType == 15)
                node.type = new BooleanType();
            else if (node.OperatorType == 9)
            {
                node.type = new NumberType();
            }

            return node.type;
        }

        //checks if both types of an edgeNode is an vertex
        public override object Visit(EdgeCreateNode node, object o)
        {
            node.LeftSide.Accept(this,null);

            if (node.LeftSide.type.ToString() == "vertex")
            {
                if (node.RightSide.Capacity > 0)
                {
                    foreach (Tuple<IdentNode, List<AssignNode>> n in node.RightSide)
                    {

                        if (symbolTable.Find(n.Item1.Identifier).Type != node.LeftSide.type)
                            parser.SemErr("the edge creating doesnt involve only vertexes.");                       
                    }
                }
                else
                    parser.SemErr("No vertex in list on edgeCreateNode");
            }
            else
                parser.SemErr("The edge is not between vertexes");

            if (node.Operator != 16 && node.Operator != 17 && node.Operator != 18)
                parser.SemErr("EdgeCreateNode is not of operatertype 16-17-18.");
            return null;
        }

        //unfinised try on checking function returntype
        public override object Visit(FuncDeclNode node, object o)
        {
            FunctionType returnType = (FunctionType)node.SymbolObject.Type;

//            if (functionReturnType is null)
//            {
//                if (returnType.ReturnType is null)
//                { }
//            }

//            else
//            {
//                if (returnType.ReturnType is null)
//                {                         parser.SemErr("ReturnType is not corrent");
//}
//                else
//                {

//                    Console.WriteLine("FuncDeclNode: " + node.SymbolObject.Name + " returnType: " + returnType.ReturnType + " functiontype: " + functionReturnType);

//                    if (returnType.ReturnType.ToString() != functionReturnType.ToString())
//                        parser.SemErr("ReturnType is not corrent");
//                }
//            }
            //Console.WriteLine("Parameters: "+node.Parameters.statements.Count);

            node.Parameters.Accept(this, null);
            node.Body.Accept(this, null);

            return null;
        }

        //Checks for default and SymbolobjectValue. If it doesnt exist, create the SymbolObject
        public override object Visit(VarDeclNode node, object o)
        {
            if (node.DefaultValue != null)
            {
                if (node.DefaultValue.Accept(this, null) != null)
                {

                    //If the value is of type number, assume it is the same as collectiontype<number>
                    if (node.SymbolObject.Type != new ListType((BaseType)node.DefaultValue.Accept(this, null))&&
                            node.SymbolObject.Type != new QueueType((BaseType)node.DefaultValue.Accept(this, null)) &&
                            node.SymbolObject.Type != new SetType((BaseType)node.DefaultValue.Accept(this, null)) &&
                            node.SymbolObject.Type != new StackType((BaseType)node.DefaultValue.Accept(this, null)) &&
                            node.DefaultValue.Accept(this, null).ToString() != node.SymbolObject.Type.ToString())
                    {
                        parser.SemErr("Default value is not the same type as Symbolobject type value.");
                    }
                }
                else
                {
                    if (node.SymbolObject.Type.ToString() != "vertex" && node.SymbolObject.Type.ToString() != "edge")
                        parser.SemErr("Default value is null. And not a vertex or edge.");
                }

            }
            symbolTable.NewObj(node.SymbolObject.Name, node.Type, node.SymbolObject.Kind);
            
            return null;
        }

        //Creates a vertex object in SymbolTable
        public override object Visit(VertexDeclNode node, object o)
        {

            symbolTable.NewObj(node.SymbolObject.Name, node.SymbolObject.Type, node.SymbolObject.Kind);
            node.Attributes.Accept(this, null);

            return null;
        }

        //Checks for an assigned value is in a correct type. like number sum = 0. Which failes if it is not a number.
        public override object Visit(AssignNode node, object o)
        {
            node.Value.Accept(this, null);
            node.Target.Accept(this, null);
            BaseType targetType = null;

            if (symbolTable.Find(node.Target.Identifier) != null)
                targetType = symbolTable.Find(node.Target.Identifier).Type;

            BaseType valueTarget = (BaseType)node.Value.Accept(this, null);

            if (node.Value.Accept(this, null) != null){            
                if (targetType != valueTarget)
                    parser.SemErr("Imcompatible types in Assign");
            }
            else
                parser.SemErr("AssignNode is null");

            return null;
        }

        //visits blocknode
        public override object Visit(BlockNode node, object o)
        {
            foreach (Node n in node.statements)
            {
                n.Accept(this, null);
            }
            return null;
        }

        //Visits foreachNode (undone)
        public override object Visit(ForeachNode node, object o)
        {
            node.IterationVar.Accept(this,null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            return node;
        }

        //Visits forNode (undone)
        public override object Visit(ForNode node, object o)
        {
            node.Initializer.Accept(this, null);
            node.Condition.Accept(this, null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            return null;
        }

        //Visits HeadNode
        public override object Visit(HeadNode node, object o)
        { 
            node.attrDeclBlock.Accept(this, null);
            return null;
        }

        //Checks if the condition is a bool
        public override object Visit(IfNode node, object o)
        {
           
            node.Condition.Accept(this, null);
            node.Body.Accept(this, null);

            if ((BaseType)node.Condition.Accept(this, null) != new BooleanType())
            {
                parser.SemErr("Expression in if is not a boolean");
            }

            if (node.Condition.Accept(this, null) == null)
            {
                //This is an else node
            }

            return null;
        }

        //visits LonecallNode
        public override object Visit(LoneCallNode node, object o)
        {  
            node.Call.Accept(this, null);

            return null;
        }

        //For functionReturnType (undone)
        public override object Visit(ReturnNode node, object o)
        { 

            node.Ret.Accept(this, null);

            functionReturnType = node.Ret.type;

            return node.Ret.type;
        }

        //visits WhileNode (undone)
        public override object Visit(WhileNode node, object o)
        {
            //Console.WriteLine("entering whileloop" + node.Condition.Accept(this, null).GetType());
            // if (!node.Condition.Accept(this, null).Equals("2"))
            //       Console.WriteLine("visitWhileLOOP");

           // Console.WriteLine("WhileNode " +node.Condition.ToString());
            //Console.WriteLine("WhileNode " + node.Body.ToString());


            node.Condition.Accept(this, null);
            node.Body.Accept(this, null);

            return node;
        }
        
        //visits MagiaNode
        public override object Visit(MAGIA node, object o)
        {
            node.block.Accept(this, null);
           
            return null;
        }

        public override object Visit(BreakNode node, object o)
        {
            return null;
        }

        public override object Visit(ContinueNode node, object o)
        {
            return null;
        }

        //visits MultiDecl
        public override object Visit(MultiDecl multiDecl, object p)
        {
            foreach (Node n in multiDecl.Decls)
            {
                n.Accept(this, null);
            }
            return null;
        }

        //prints symboltable (Used for bugfixising, unused at the moment)
        public void Print()
        {
            string output = "";


            foreach (KeyValuePair<string, Obj> kvp in symbolTable.GetDic())
            {
                output += string.Format("{0} + {1}", kvp.Key, kvp.Value.Type);
                output += "\n";
            }
            output += "\n\n\n";

            foreach (SymbolTable table in symbolTable.GetScopes())
            {

                foreach (KeyValuePair<string, Obj> kvp in table.GetDic())
                {
                    output += string.Format("{0} + {1}", kvp.Key, kvp.Value.Type);
                    output += "\n";
                }
                output += "\n\n\n";
            }
            Console.WriteLine(output);
        }
    }
}
