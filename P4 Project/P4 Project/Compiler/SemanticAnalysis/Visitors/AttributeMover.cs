﻿using System.Collections.Generic;
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
    public class AttributeMover : Visitor
    {
        //This Visitor will fix attributes and functions
        //Like:
        //1. Remove the function "SymbolObject" and set it as type on function scope.
        //2. Move attributes from top scope to their own special scope.
        //3. Add the defualt attributes
        //4. Add the defualt functions
        public override string AppropriateFileName { get; } = "AttributeMover.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public override SymTable Table { get; set; }
        public AttributeMover(SymTable Table) {
            this.Table = Table;
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
            node.Expressions.ForEach(n=>n.Accept(this));
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
            //1. Remove the function "SymbolObject" and set it as type on function scope.
            Table.GetScopes().ForEach(s => {
                if (s.name == node.SymbolObject.Name)
                {
                    s.type = node.SymbolObject.type;
                    Table.RemoveObj(node.SymbolObject);
                }
            });
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
            //2. Move attributes from top scope to their own special scope.

            //Add defualt attributes
            if (node.type.name == "vertex")
            {
                PreDefined.preDefinedAttributesVertex.ForEach(va =>
                {
                    Table.vertexAttr.AddObj(new Obj(va, new BaseType(PreDefined.getTypeOfAttribute(va)), 0, null));
                });
                //Add user defined attributes
                node.attrDeclBlock.Statements.ForEach(s => {
                    VarDeclNode v = (VarDeclNode)s;
                    Table.vertexAttr.AddObj(v.SymbolObject);
                    Table.RemoveObj(v.SymbolObject);
                });
            }
            else if (node.type.name == "edge")
            {
                PreDefined.preDefinedAttributesEdge.ForEach(va =>
                {
                    Table.edgeAttr.AddObj(new Obj(va, new BaseType(PreDefined.getTypeOfAttribute(va)), 0, null));
                });
                //Add user defined attributes
                node.attrDeclBlock.Statements.ForEach(s => {
                    VarDeclNode v = (VarDeclNode)s;
                    Table.edgeAttr.AddObj(v.SymbolObject);
                    Table.RemoveObj(v.SymbolObject);
                });
            }

            
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
            //Creates a scope just for header attributes
            SymTable v = new SymTable(null, null, " ");
            v.header = true;
            Table.vertexAttr = v;
            SymTable e = new SymTable(null, null, " ");
            e.header = true;
            Table.edgeAttr = e;
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