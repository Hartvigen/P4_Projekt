﻿using System;
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

namespace P4_Project.Visitors
{
    class TypeVisitor : Visitor
    {
        public SymbolTable symbolTable;

        public TypeVisitor(SymbolTable table)
        {
            symbolTable = table;
        }


        public override object Visit(CallNode node, object o)
        {
          //  node.Accept(this, null);

            node.Parameters.Accept(this,null);
            return null;
        }

        public override object Visit(VarNode node,object o)
        {
           // node.Accept(this, null);
            return null;
        }

        public override object Visit(BoolConst node, object o)
        {
            //node.Accept(this, null);

            return node.type = new BooleanType();
        }

        public override object Visit(CollecConst node, object o)
        {
         //   node.Accept(this, null);

            return null;
        }

        public override object Visit(NoneConst node, object o)
        {
         //   node.Accept(this, null);

            return null;
        }

        public override object Visit(NumConst node, object o)
        {
         //   node.Accept(this, null);

            return node.type = new NumberType();
        }

        public override object Visit(TextConst node, object o)
        {
            return node.type = new TextType();
        }

        public override object Visit(BinExprNode node, object o)
        {
 
            node.Left.Accept(this, null);
            node.Right.Accept(this, null);



            Console.WriteLine("left: " + node.Left.Accept(this, null));
            Console.WriteLine("right: " + node.Right.Accept(this, null));


            if (node.Left.Accept(this, null) != null && node.Right.Accept(this, null) != null)
            {

               
                if ((BaseType) node.Left.Accept(this, null) == (BaseType) node.Right.Accept(this, null))
                {

                    Console.WriteLine("BinExprNode is matching");
                }
                else
                    Console.WriteLine("BinExprNode is not matching");
            }
            else
                Console.WriteLine("BinExprNode is null");

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
            if (node.OperatorType == 9 || node.OperatorType == 10 || node.OperatorType == 11 || node.OperatorType == 12 || node.OperatorType == 13 || node.OperatorType == 14)
                node.type = new NumberType();
            else
            {
                node.type = new BooleanType();
            }

            Console.WriteLine("UnaExprNode: " + node.type);

            return node.type;
        }

        public override object Visit(EdgeCreateNode node, object o)
        {
            node.LeftSide.Accept(this,null);
           // node.RightSide.Accept(this,null);
            node.LeftSide.Accept(this,null);  
            return null;
        }

        public override object Visit(FuncDeclNode node, object o)
        {
            Console.WriteLine("FuncDeclNode: " + node.SymbolObject.Name + " " + node.SymbolObject.Type);

            Console.WriteLine("Parameters: "+node.Parameters.statements.Count);
            node.Parameters.Accept(this, null);
            node.Body.Accept(this, null);



            return null;
        }

        public override object Visit(VarDeclNode node, object o)
        {

           // node.DefaultValue.Accept(this, null);

           Console.WriteLine("VarDeclNode: " + node.SymbolObject.Name + " " + node.SymbolObject.Type);

            //if (symbolTable.Find(node.SymbolObject.Name)== null)
            symbolTable.NewObj(node.SymbolObject.Name, node.Type, node.SymbolObject.Kind);
            
            return null;
        }

        public override object Visit(VertexDeclNode node, object o)
        {  
            node.Attributes.Accept(this, null);
            return null;
        }

        public override object Visit(AssignNode node, object o)
        {
            node.Value.Accept(this, null);
            node.Target.Accept(this, null);
            //Print();

            //BaseType tType = node.Value.type;
            //BaseType vType = node.Value.type;

            //Console.WriteLine("-----------------AssignNode target: " + node.Target.Identifier);

            //Console.WriteLine("AssignNode target: " + symbolTable.Find(node.Target.Identifier).Type);
            BaseType targetType = symbolTable.Find(node.Target.Identifier).Type;
            BaseType valueTarget = (BaseType)node.Value.Accept(this, null);

            if (node.Value.Accept(this, null) != null){            
                if (targetType != valueTarget)
                    Console.WriteLine("Imcompatible types in Assign");
                else
                    Console.WriteLine("AssignNode " +targetType + " == "+ valueTarget + " Success");
            }
            // Console.WriteLine("TARGET = "+node.Target.ToString() + " Value = " + node.Value.ToString());

            return null;
        }

        public override object Visit(BlockNode node, object o)
        { 
            foreach (Node n in node.statements)
                n.Accept(this, null);

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


            //Console.WriteLine("IfNode body "+node.Body.Accept(this, null));
            //Console.WriteLine("IfNode ElseNode "+node.ElseNode.Accept(this, null));

            // Type eType = (Type)node.Condition.Accept(this, null);

            //if (node.Body.Accept(this, null) != node.ElseNode.Accept(this, null))
            //    Console.WriteLine("Expression in if is not a boolean");


            

            return null;
        }

        public override object Visit(LoneCallNode node, object o)
        {  
            node.Call.Accept(this, null);
            return null;
        }

        public override object Visit(ReturnNode node, object o)
        { 
            node.Ret.Accept(this, null);
            return null;
        }

        public override object Visit(WhileNode node, object o)
        {
            //Console.WriteLine("entering whileloop" + node.Condition.Accept(this, null).GetType());
            // if (!node.Condition.Accept(this, null).Equals("2"))
            //       Console.WriteLine("visitWhileLOOP");

            Console.WriteLine("WhileNode " +node.Condition.ToString());
            //Console.WriteLine("WhileNode " + node.Body.ToString());


            node.Condition.Accept(this, null);
            node.Body.Accept(this, null);

            return node;
        }
        
        public override object Visit(MAGIA node, object o)
        {



            //string output = "";


            //foreach (KeyValuePair<string, Obj> kvp in symbolTable.GetDic())
            //{
            //    output += string.Format("{0} + {1}", kvp.Key, kvp.Value.Type);
            //    output += "\n";
            //}
            //output += "\n\n\n";

            //foreach (SymbolTable table in symbolTable.GetScopes())
            //{

            //    foreach (KeyValuePair<string, Obj> kvp in table.GetDic())
            //    {
            //        output += string.Format("{0} + {1}", kvp.Key, kvp.Value.Type);
            //        output += "\n";
            //    }
            //    output += "\n\n\n";
            //}
            //Console.WriteLine(output);


            node.block.Accept(this, null);
           // Print();
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
