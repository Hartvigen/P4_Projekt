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
using P4_Project.Types.Collections;
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
            node.Parameters.Accept(this,null);
            return null;
        }

        public override object Visit(VarNode node,object o)
        {
            return null;
        }

        public override object Visit(BoolConst node, object o)
        {
            return null;
        }

        public override object Visit(CollecConst node, object o)
        {
            return null;
        }

        public override object Visit(NoneConst node, object o)
        {
            return null;
        }

        public override object Visit(NumConst node, object o)
        {

            return null;
        }

        public override object Visit(TextConst node, object o)
        {
            return null;
        }

        public override object Visit(BinExprNode node, object o)
        {
 
            node.Left.Accept(this, null);
            node.Right.Accept(this, null);
            return null;

        }

        public override object Visit(UnaExprNode node, object o)
        {
            node.Expr.Accept(this, null);
            return null;
        }

        public override object Visit(EdgeCreateNode node, object o)
        {
            //node.Start.Accept(this);
            //node.End.Accept(this);
            //node.Attributes.Accept(this);  
            return null;
        }

        public override object Visit(FuncDeclNode node, object o)
        {
            node.Parameters.Accept(this, null);
            node.Body.Accept(this, null);
            return null;
        }

        public override object Visit(VarDeclNode node, object o)
        {

            //node.DefaultValue.Accept(this, null);
            //symbolTable.NewObj(node.SymbolObject.Name, SetType(node), 0);

            return null;
        }

        public override object Visit(VertexDeclNode node, object o)
        {  
            node.Attributes.Accept(this, null);
            return null;
        }

        public override object Visit(AssignNode node, object o)
        {
            Type tType = (Type) node.Target.Accept(this, null);
            Type vType = (Type) node.Value.Accept(this, null);

           if (!node.Value.GetType().Equals(null))
                Console.WriteLine("Value is not a variable");

            if (!tType.Equals(vType))
                Console.WriteLine("Imcompatible types in Assign");
            else
                Console.WriteLine("Success");

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
            
            Type eType = (Type)node.Condition.Accept(this, null);

           // if (!eType.Equals(TypeS.boolean))
           //     Console.WriteLine("Expression in if is not a boolean");


            node.Body.Accept(this, null);
            node.ElseNode.Accept(this, null);

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


            node.Condition.Accept(this, null);
            node.Body.Accept(this, null);

            return node;
        }
        
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

        public override object Visit(MultiDecl multiDecl, object p)
        {
            return null;
        }
    }
}
