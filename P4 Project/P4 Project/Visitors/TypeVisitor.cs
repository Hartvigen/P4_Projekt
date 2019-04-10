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
using static P4_Project.TypeS;

namespace P4_Project.Visitors
{
    class TypeVisitor : Visitor
    {
        SymbolTable symbolTable;

        public TypeVisitor(SymbolTable table)
        {
            symbolTable = table;
        }


        public override void Visit(CallNode node)
        { 
            node.Parameters.Accept(this);
        }

        public override void Visit(VarNode node)
        {

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
            //Console.WriteLine(string.Format("{0} {1} ", node.Left.GetType(), node.Right.GetType()));
        }

        public override void Visit(UnaExprNode node)
        {
            node.Expr.Accept(this);  
        }

        public override void Visit(EdgeCreateNode node)
        {
            //node.Start.Accept(this);
            //node.End.Accept(this);
            //node.Attributes.Accept(this);  
        }

        public override void Visit(FuncDeclNode node)
        {
            node.Parameters.Accept(this);
            node.Body.Accept(this);
        }

        public override void Visit(VarDeclNode node)
        {

            if (node.DefaultValue != null)
            {
                node.DefaultValue.Accept(this);
                Console.WriteLine(string.Format("{0} = {1} ", node.GetVarType(), node.DefaultValue));
            }
                
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
            foreach (Node n in node.statements)
                n.Accept(this);    
        }

        public override void Visit(ForeachNode node)
        {
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
        }

        public override void Visit(ForNode node)
        {
            //Tjek om alle delene af forløkken er korrekt
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
            // Om den ikker er null er giver en bool, ligesom while
            if (node.Condition != null)
                node.Condition.Accept(this);
            node.Body.Accept(this);
            if (node.ElseNode != null)
                node.ElseNode.Accept(this);
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
            // Her skal der tjekkes om hver side af conditionen er samme type
            node.Condition.Accept(this);
            node.Body.Accept(this);
        }

        public override void Visit(MAGIA node)
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
            //throw new NotImplementedException();
        }
    }
}
