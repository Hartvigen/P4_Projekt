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

namespace P4_Project.Visitors
{
    class XmlTreeBuilder : Visitor
    {
        public StringBuilder ast = new StringBuilder();

        //All the functions does the same thing:
        //1. Start XML tag of type whatever node type is.
        //2. Accept the node
        //3. End XML tag of whatever node type is.
        //That will generate a XML tree that shows the entire node structure of the program.
        public override void Visit(CallNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.parameters.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(IdentNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(MemberNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.source.Accept(this);
            node.memberIdent.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VarNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BoolConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(CollecConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(NoneConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(NumConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(TextConst node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BinExprNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.left.Accept(this);
            node.right.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(UnaExprNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.expr.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(EdgeDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.start.Accept(this);
            node.end.Accept(this);
            node.attributes.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(FuncDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.parameters.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VarDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            if(node.expr != null)
            node.expr.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(VertexDeclNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.attributes.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(AssignNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.target.Accept(this);
            node.value.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(Block node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            foreach (Node n in node.statements)
                n.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForeachNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.iterationVar.Accept(this);
            node.iterator.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ForNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.initializer.Accept(this);
            node.condition.Accept(this);
            node.iterator.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(HeadNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.attrDeclBlock.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(IfNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            if(node.condition != null)
            node.condition.Accept(this);
            node.body.Accept(this);
            if(node.elseNode != null)
            node.elseNode.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(LoneCallNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.call.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ReturnNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.ret.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(WhileNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.condition.Accept(this);
            node.body.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(MAGIA node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            node.block.Accept(this);
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(BreakNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }

        public override void Visit(ContinueNode node)
        {
            ast.AppendLine($"<{node.GetType().Name}>");
            ast.AppendLine($"</{node.GetType().Name}>");
        }
    }
}