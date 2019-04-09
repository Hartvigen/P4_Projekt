using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.SymTab;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "FuncDeclNode" represents the declaration of a function.
    /// </summary>
    public class FuncDeclNode : DeclNode
    {
        public BlockNode Parameters { get; private set; }
        public BlockNode Body { get; private set; }


        public FuncDeclNode() { }

        public FuncDeclNode(Obj symbolObject, BlockNode parameters, BlockNode body) 
            : base(symbolObject)
        {
            Parameters = parameters;
            Body = body;
        }


        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
