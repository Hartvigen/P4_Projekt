using System;
using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.SymbolTable;

namespace P4_Project.Compiler.SemanticAnalysis.Visitors
{
    public sealed class AttributeMover : Visitor
    {
        //This Visitor will fix attributes and functions
        //Like:
        //1. Remove the function "SymbolObject" and set it as type on function scope.
        //2. Move attributes from top scope to their own special scope.
        //3. Add the default attributes
        //4. Add the default functions
        public override string AppropriateFileName { get; } = "AttributeMover.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        private SymTable Table { get; }
        public AttributeMover(SymTable table) {
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
                    s.type = node.SymbolObject.Type;
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

            switch (node.type.name)
            {
                //Add default attributes
                case "vertex":
                    //Add user defined attributes
                    node.attrDeclBlock.Statements.ForEach(s => {
                        var v = (VarDeclNode)s;
                        Table.vertexAttr.AddObj(v.SymbolObject);
                        if(Table.GetDic().ContainsKey(v.SymbolObject.Name))
                            Table.RemoveObj(v.SymbolObject);
                    });
                    break;
                case "edge":
                    //Add user defined attributes
                    node.attrDeclBlock.Statements.ForEach(s => {
                        var v = (VarDeclNode)s;
                        Table.edgeAttr.AddObj(v.SymbolObject);
                        Table.RemoveObj(v.SymbolObject);
                    });
                    break;
                default: throw new Exception(node.type.name + " is not a valid type for a HeadNode");
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
            var v = new SymTable(null, null, " ") {header = true};
            Table.vertexAttr = v;
            PreDefined.PreDefinedAttributesVertex.ForEach(va =>
            {
                Table.vertexAttr.AddObj(new Obj(va, new BaseType(PreDefined.GetTypeOfPreDefinedAttributeVertex(va)), 0));
            });
            var e = new SymTable(null, null, " ") {header = true};
            Table.edgeAttr = e;
            PreDefined.PreDefinedAttributesEdge.ForEach(va =>
            {
                Table.edgeAttr.AddObj(new Obj(va, new BaseType(PreDefined.GetTypeOfPreDefinedAttributeEdge(va)), 0));
            });
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
