using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Types;

namespace P4_Project.Visitors
{
    public abstract class Visitor
    {
        public abstract string AppropriateFileName { get;}
        public abstract StringBuilder Result { get; }
        public abstract List<string> ErrorList { get; }

        //Identifier
        public abstract BaseType Visit(CallNode node);
        public abstract BaseType Visit(VarNode node);

        public abstract BaseType Visit(MultiDecl multiDecl);

        //Values
        public abstract BaseType Visit(BoolConst node);
        public abstract BaseType Visit(CollecConst node);
        public abstract BaseType Visit(NoneConst node);
        public abstract BaseType Visit(NumConst node);
        public abstract BaseType Visit(TextConst node);

        //Expressions
        public abstract BaseType Visit(BinExprNode node);
        public abstract BaseType Visit(UnaExprNode node);

        //Decls
        public abstract BaseType Visit(EdgeCreateNode node);
        public abstract BaseType Visit(FuncDeclNode node);
        public abstract BaseType Visit(VarDeclNode node);
        public abstract BaseType Visit(VertexDeclNode node);

        //Stmts
        public abstract BaseType Visit(AssignNode node);
        public abstract BaseType Visit(BlockNode node);
        public abstract BaseType Visit(ForeachNode node);
        public abstract BaseType Visit(ForNode node);
        public abstract BaseType Visit(HeadNode node);
        public abstract BaseType Visit(IfNode node);
        public abstract BaseType Visit(LoneCallNode node);
        public abstract BaseType Visit(ReturnNode node);
        public abstract BaseType Visit(WhileNode node);
        public abstract BaseType Visit(BreakNode node);
        public abstract BaseType Visit(ContinueNode node);

        //AST
        public abstract BaseType Visit(Magia node);
    }
}
