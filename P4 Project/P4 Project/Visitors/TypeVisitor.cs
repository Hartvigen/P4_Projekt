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
        public SymbolTable symbolTable;
        public Parser parser;

        public TypeVisitor(Parser parserObject)
        {
            symbolTable = parserObject.tab;
            parser = parserObject;
        }


        public override object Visit(CallNode node, object o)
        {
            //  node.Accept(this, null);
            //Console.WriteLine("CallNode " + node.Identifier);
            BaseType type = null;
            FunctionType returnType;

            if (node.Identifier != "Edge" && node.Identifier != "vertices" && node.Identifier != "clear" && node.Identifier != "removeEdge" && node.Identifier != "ClearEdges" && node.Identifier != "ClearVertices" && node.Identifier != "ClearAll") {
                //node.type = (BaseType)new NumberType();
                returnType = (FunctionType)symbolTable.Find(node.Identifier).Type;
                type = returnType.returnType();
                

            }
            if(node.Identifier == "vertices")
            {
                type = new SetType(new VertexType());
            }

            node.Parameters.Accept(this,null);
            return type;
        }

        public override object Visit(VarNode node,object o)
        {
            node.type = symbolTable.Find(node.Identifier).Type;
            
           // Console.WriteLine("VARNODE: " + node.Identifier + " " + node.type);


            // symbolTable.NewObj(node.SymbolObject.Name, node.Type, node.SymbolObject.Kind);
            return node.type;
        }

        public override object Visit(BoolConst node, object o)
        {
            //node.Accept(this, null);
            //Console.WriteLine("BoolConst: " + node.Value);
            
            return node.type = new BooleanType();
        }

        public override object Visit(CollecConst node, object o)
        {
            int i = 0;
            Console.WriteLine("CollecConst nodetype: " + node.ToString());

            

            foreach (ExprNode n in node.Expressions)
            {
                n.Accept(this, null);
                node.type = n.type;
                Console.WriteLine("CollecConst: " + n.type);
                i++;
            }
            if (i > 0)
            {
                Console.WriteLine("----------------------");

            }
            else
                return null;
            return node.type;//node.type;<------------------------------------------------------et problem
        }

        public override object Visit(NoneConst node, object o)
        {
            Console.WriteLine("NoneConst: "  + node.type);

            //   node.Accept(this, null);

            return node.type;
        }

        public override object Visit(NumConst node, object o)
        {
             // node.Accept(this, null);
            //Console.WriteLine("NumConst: " +  node.Value + node.type);

            if (node != null)
                return node.type = new NumberType();
            else
            {
                parser.SemErr("NumConst is not a number");
                return null;
            }
        }

        public override object Visit(TextConst node, object o)
        {
            //node.Accept(this, null);
            //Console.WriteLine("TextConst: " + node.Value + node.type);

           //Console.WriteLine("TextConst: " + node.type);
            if (node != null)
                return node.type = new TextType();
            else
            {
                parser.SemErr("TextConst is not a text");
                return null;
            }
        }

        public override object Visit(BinExprNode node, object o)
        {
 
            node.Left.Accept(this, null);
            node.Right.Accept(this, null);

            //Console.WriteLine("left: " + node.Left.Accept(this, null));
            //Console.WriteLine("right: " + node.Right.Accept(this, null));


            if (node.Left.Accept(this, null) != null && node.Right.Accept(this, null) != null)
            {


                if ((BaseType)node.Left.Accept(this, null) != (BaseType)node.Right.Accept(this, null)) { 
                    //Console.WriteLine("BinExprNode is not matching---"+ (BaseType)node.Left.Accept(this, null)+ " == " +(BaseType)node.Right.Accept(this, null)+"----------------------------------------------------------");
                    parser.SemErr("BinExprNode is not matching");
                }
            }
            else
                parser.SemErr("BinExprNode is null");

            if (node.OperatorType == 9 || node.OperatorType == 10 || node.OperatorType == 11 || node.OperatorType == 12 || node.OperatorType == 13 || node.OperatorType == 14)
                node.type = new NumberType();
            else
            {
                node.type = new BooleanType();
            }

            // node.type = (BaseType)Convert.ChangeType(node.type, TypeCode.Boolean);
            // Console.WriteLine("BinExprNode: " + node.type);

            return node.type;
        }
        

        public override object Visit(UnaExprNode node, object o)
        {
            node.Expr.Accept(this, null);

            if (node.OperatorType == 15)
                node.type = new BooleanType();
            else if (node.OperatorType == 9)
            {
                node.type = new NumberType();
            }

            //Console.WriteLine("UnaExprNode: " + node.type);

            return node.type;
        }

        public override object Visit(EdgeCreateNode node, object o)
        {
            node.LeftSide.Accept(this,null);



            //node.RightSide.Accept(this,null);
           // node.LeftSide.Accept(this,null);  
            return null;
        }

        public override object Visit(FuncDeclNode node, object o)
        {
           // Console.WriteLine("FuncDeclNode: " + node.SymbolObject.Name + " " + node.SymbolObject.Type);

            //Console.WriteLine("Parameters: "+node.Parameters.statements.Count);
            node.Parameters.Accept(this, null);
            node.Body.Accept(this, null);



            return null;
        }

        public override object Visit(VarDeclNode node, object o)
        {
            // Console.WriteLine("1VarDeclNode: " + node.SymbolObject.Name + " " + node.SymbolObject.Type);
            //BaseType test = (BaseType)node.DefaultValue.Accept(this, null);
            if (node.DefaultValue != null)
            {
                if (node.DefaultValue.Accept(this, null) != null)
                {

                    Console.WriteLine("DefaultValue: " + node.DefaultValue.Accept(this, null) + " SymbolObject: " + node.SymbolObject.Type.ToString());


                    if (node.SymbolObject.Type != new ListType((BaseType)node.DefaultValue.Accept(this, null))||
                            node.SymbolObject.Type != new QueueType((BaseType)node.DefaultValue.Accept(this, null)) ||
                            node.SymbolObject.Type != new SetType((BaseType)node.DefaultValue.Accept(this, null)) ||
                            node.SymbolObject.Type != new StackType((BaseType)node.DefaultValue.Accept(this, null)))
                    {
                        parser.SemErr("Default value is not the same type as Collection type value.");
                    }
                    else if (node.DefaultValue.Accept(this, null).ToString() != node.SymbolObject.Type.ToString())
                        parser.SemErr("Default value is not the same type as SymbolObject value.");

                }
                else
                {
                    Console.WriteLine("DefaultValue: " + node.DefaultValue.Accept(this, null) + " SymbolObject: " + node.SymbolObject.Type.ToString());

                    if (node.SymbolObject.Type.ToString() != "vertex" && node.SymbolObject.Type.ToString() != "edge")
                        parser.SemErr("Default value is null. And not a vertex or edge.");
                }

            }
            symbolTable.NewObj(node.SymbolObject.Name, node.Type, node.SymbolObject.Kind);
            
            return null;
        }

        public override object Visit(VertexDeclNode node, object o)
        {
           // Console.WriteLine("VertexDeclNode: " +node.SymbolObject.Name + " " + node.SymbolObject.Type);


            //Console.WriteLine("VarDeclNode: " + node.SymbolObject.Name + " " + node.SymbolObject.Type);

            //if (symbolTable.Find(node.SymbolObject.Name)== null)
            symbolTable.NewObj(node.SymbolObject.Name, node.SymbolObject.Type, node.SymbolObject.Kind);

      

            node.Attributes.Accept(this, null);
            return null;// new VertexType();
        }

        public override object Visit(AssignNode node, object o)
        {
            node.Value.Accept(this, null);
            node.Target.Accept(this, null);
            //Print();

            //BaseType tType = node.Value.type;
            //BaseType vType = node.Value.type;
            
            BaseType targetType = symbolTable.Find(node.Target.Identifier).Type;
            BaseType valueTarget = (BaseType)node.Value.Accept(this, null);

            if (node.Value.Accept(this, null) != null){            
                if (targetType != valueTarget)
                    parser.SemErr("Imcompatible types in Assign");
            }
            else
                parser.SemErr("AssignNode is null");

           Console.WriteLine(node.Target.Identifier);
            //Console.WriteLine("TARGET = "+ symbolTable.Find(node.Target.Identifier).Type + " Value = " + (BaseType)node.Value.Accept(this, null));

            return null;
        }

        public override object Visit(BlockNode node, object o)
        {
            foreach (Node n in node.statements) { 
                //Console.WriteLine("BlockNode: " + n);
                n.Accept(this, null);
            }
            return null;
        }

        public override object Visit(ForeachNode node, object o)
        {
            node.IterationVar.Accept(this,null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            return node;
        }

        public override object Visit(ForNode node, object o)
        {
            node.Initializer.Accept(this, null);
            node.Condition.Accept(this, null);
            node.Iterator.Accept(this, null);
            node.Body.Accept(this, null);
            return null;
        }

        public override object Visit(HeadNode node, object o)
        { 
            
            node.attrDeclBlock.Accept(this, null);
            return null;
        }

        public override object Visit(IfNode node, object o)
        {
           

           // Console.WriteLine("IfNode Condition "+node.Condition.Accept(this, null));

            node.Condition.Accept(this, null);
            //node.ElseNode.Accept(this, null);
            node.Body.Accept(this, null);

            //Console.WriteLine("IfNode Condition " + node.ElseNode.Accept(this, null));


            //Console.WriteLine("IfNode ElseNode "+node.ElseNode.Accept(this, null));

            // Type eType = (Type)node.Condition.Accept(this, null);

            if ((BaseType)node.Condition.Accept(this, null) != new BooleanType())
            {
                parser.SemErr("Expression in if is not a boolean");

                //Console.WriteLine("Expression in if is not a boolean");
            }

            if (node.Condition.Accept(this, null) == null)
            {
                //Console.WriteLine("This is an else node");
            }

            //if (node.ElseNode.Accept(this, null) != null ){
            //   if((BaseType)node.ElseNode.Accept(this, null) != new BooleanType())
            //        Console.WriteLine("Expression in ElseIf is not a boolean");
            //}

            

            return null;
        }

        public override object Visit(LoneCallNode node, object o)
        {  
            node.Call.Accept(this, null);
           // Console.WriteLine("LoneCallNode " + node.Call.Identifier);
            //node.Call.type = symbolTable.Find(node.Call.Identifier).Type;
            return null;
        }

        public override object Visit(ReturnNode node, object o)
        { 

            node.Ret.Accept(this, null);
          //  Console.WriteLine("ReturnNode " + node.Ret.type);

            return null;
        }

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
        
        public override object Visit(MAGIA node, object o)
        {
           // Print();
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

        public override object Visit(MultiDecl multiDecl, object p)
        {
            foreach (Node n in multiDecl.Decls)
            {
                //Console.WriteLine("MultiDecl: " + n);
                n.Accept(this, null);
            }
            return null;
        }

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
