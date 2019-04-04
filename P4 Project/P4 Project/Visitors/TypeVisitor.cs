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
            node.parameters.Accept(this);
        }

        public override void Visit(IdentNode node)
        {
            
        }

        public override void Visit(MemberNode node)
        {    
            node.source.Accept(this);
            node.memberIdent.Accept(this);   
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
            node.left.Accept(this);
            node.right.Accept(this);      
        }

        public override void Visit(UnaExprNode node)
        {
            node.expr.Accept(this);  
        }

        public override void Visit(EdgeDeclNode node)
        {
            node.start.Accept(this);
            node.end.Accept(this);
            node.attributes.Accept(this);  
        }

        public override void Visit(FuncDeclNode node)
        {
            node.parameters.Accept(this);
            node.body.Accept(this);
        }

        public override void Visit(VarDeclNode node)
        {

            if (node.expr != null)
                node.expr.Accept(this);
        }

        public override void Visit(VertexDeclNode node)
        {  
            node.attributes.Accept(this);
        }

        public override void Visit(AssignNode node)
        {
            node.target.Accept(this);
            node.value.Accept(this);          
        }

        public override void Visit(Block node)
        { 
            foreach (Node n in node.statements)
                n.Accept(this);    
        }

        public override void Visit(ForeachNode node)
        {
            node.iterationVar.Accept(this);
            node.iterator.Accept(this);
            node.body.Accept(this);
        }

        public override void Visit(ForNode node)
        {
            node.initializer.Accept(this);
            node.condition.Accept(this);
            node.iterator.Accept(this);
            node.body.Accept(this);
        }

        public override void Visit(HeadNode node)
        { 
            node.attrDeclBlock.Accept(this); 
        }

        public override void Visit(IfNode node)
        {
            if (node.condition != null)
                node.condition.Accept(this);
            node.body.Accept(this);
            if (node.elseNode != null)
                node.elseNode.Accept(this);
        }

        public override void Visit(LoneCallNode node)
        {  
            node.call.Accept(this);   
        }

        public override void Visit(ReturnNode node)
        { 
            node.ret.Accept(this);    
        }

        public override void Visit(WhileNode node)
        {
            node.condition.Accept(this);
            node.body.Accept(this);
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
    }
}
