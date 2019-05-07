using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.SymbolTable;

namespace P4_Project.Visitors
{
    public class ScopeVisitor : Visitor
    {
        public override string AppropriateFileName { get; } = "ScopeErrors.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public override SymTable Table { get; set; }

        public ScopeVisitor(SymTable table)
        {
            Table = table;
        }

        public override void Visit(CallNode node)
        {
            
            node.Parameters.Accept(this);
            
        }
        public override void Visit(VarNode node)
        {
            
            node.Source?.Accept(this);
            
        }

        public override void Visit(BoolConst node)
        {
            
        }

        public override void Visit(CollecConst node)
        {
            
        }

        public override void Visit(NoneConst node)
        {
            
        }

        public override void Visit(NumConst node)
        {
            
        }

        public override void Visit(TextConst node)
        {
            
        }

        public override void Visit(BinExprNode node)
        {
            
            node.Left.Accept(this);
            node.Right.Accept(this);
            
        }

        public override void Visit(UnaExprNode node)
        {
            
            node.Expr.Accept(this);
            
        }

        public override void Visit(EdgeCreateNode node)
        {
            
            node.LeftSide.Accept(this);
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });
            
        }

        public override void Visit(FuncDeclNode node)
        {
            
            node.Parameters.Accept(this);
            node.Body.Accept(this);
            
        }

        public override void Visit(VarDeclNode node)
        {
            
            node.DefaultValue?.Accept(this);
            
        }

        public override void Visit(VertexDeclNode node)
        {
            
            node.Attributes.Accept(this);
            
        }

        public override void Visit(AssignNode node)
        {
            
            node.Target.Accept(this);
            node.Value.Accept(this);
            
        }

        public override void Visit(BlockNode node)
        {
            
            node.Statements.ForEach(n => n.Accept(this));
            
        }

        public override void Visit(ForeachNode node)
        {
            
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            
        }

        public override void Visit(ForNode node)
        {
            
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
            
        }

        public override void Visit(HeadNode node)
        {
            
            node.attrDeclBlock.Accept(this);
            
        }

        public override void Visit(IfNode node)
        {
            
            node.Condition?.Accept(this);
            node.Body.Accept(this);
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
            
            node.Condition.Accept(this);
            node.Body.Accept(this);
            
        }

        public override void Visit(Magia node)
        {
            
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
    }
}