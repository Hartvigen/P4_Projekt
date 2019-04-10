using System;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Visitors
{
    public abstract class Visitor
    {
        //Identifier
        public abstract object Visit(CallNode node, object o);
        public abstract object Visit(VarNode node, object o);

        internal void Visit(MultiDecl multiDecl, object p)
        {
            
        }

        //Values
        public abstract object Visit(BoolConst node, object o);
        public abstract object Visit(CollecConst node, object o);
        public abstract object Visit(NoneConst node, object o);
        public abstract object Visit(NumConst node, object o);
        public abstract object Visit(TextConst node, object o);

        //Expressions
        public abstract object Visit(BinExprNode node, object o);
        public abstract object Visit(UnaExprNode node, object o);

        //Decls
        public abstract object Visit(EdgeCreateNode node, object o);
        public abstract object Visit(FuncDeclNode node, object o);
        public abstract object Visit(VarDeclNode node, object o);
        public abstract object Visit(VertexDeclNode node, object o);

        //Stmts
        public abstract object Visit(AssignNode node, object o);
        public abstract object Visit(BlockNode node, object o);
        public abstract object Visit(ForeachNode node, object o);
        public abstract object Visit(ForNode node, object o);
        public abstract object Visit(HeadNode node, object o);
        public abstract object Visit(IfNode node, object o);
        public abstract object Visit(LoneCallNode node, object o);
        public abstract object Visit(ReturnNode node, object o);
        public abstract object Visit(WhileNode node, object o);
        public abstract object Visit(BreakNode node, object o);
        public abstract object Visit(ContinueNode node, object o);

        //AST
        public abstract object Visit(MAGIA node, object o);
    }
}
