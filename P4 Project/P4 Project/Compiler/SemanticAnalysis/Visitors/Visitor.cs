﻿using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Compiler.SemanticAnalysis.Visitors
{
    public abstract class Visitor
    {
        public abstract string AppropriateFileName { get;}
        public abstract StringBuilder Result { get; }
        public abstract List<string> ErrorList { get; }

        //Identifier
        public abstract void Visit(CallNode node);
        public abstract void Visit(VarNode node);

        public abstract void Visit(MultiDecl node);

        //Values
        public abstract void Visit(BoolConst node);
        public abstract void Visit(CollecConst node);
        public abstract void Visit(NoneConst node);
        public abstract void Visit(NumConst node);
        public abstract void Visit(TextConst node);

        //Expressions
        public abstract void Visit(BinExprNode node);
        public abstract void Visit(UnaExprNode node);

        //Decls
        public abstract void Visit(EdgeCreateNode node);
        public abstract void Visit(FuncDeclNode node);
        public abstract void Visit(VarDeclNode node);
        public abstract void Visit(VertexDeclNode node);

        //Stmts
        public abstract void Visit(AssignNode node);
        public abstract void Visit(BlockNode node);
        public abstract void Visit(ForeachNode node);
        public abstract void Visit(ForNode node);
        public abstract void Visit(HeadNode node);
        public abstract void Visit(IfNode node);
        public abstract void Visit(LoneCallNode node);
        public abstract void Visit(ReturnNode node);
        public abstract void Visit(WhileNode node);
        public abstract void Visit(BreakNode node);
        public abstract void Visit(ContinueNode node);

        //AST
        public abstract void Visit(Magia node);
    }
}
