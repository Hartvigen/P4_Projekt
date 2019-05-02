﻿using P4_Project.SymbolTable;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "FuncDeclNode" represents the declaration of a function.
    /// </summary>
    public class FuncDeclNode : DeclNode
    {
        public BlockNode Parameters { get; }
        public BlockNode Body { get; }

        public string returnType;

        public FuncDeclNode(Obj symbolObject, BlockNode parameters, BlockNode body) 
            : base(symbolObject)
        {
            Parameters = parameters;
            Body = body;
        }

        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }
    }
}
