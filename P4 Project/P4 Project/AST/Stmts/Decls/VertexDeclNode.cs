using P4_Project.AST.Expressions;
using P4_Project.Types;
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
    /// The "VertexDeclNode" represents the declaration of a vertex.
    /// </summary>
    public class VertexDeclNode : DeclNode
    {
        public BlockNode Attributes { get; private set; } = new BlockNode();


        public VertexDeclNode() 
        { }

        public VertexDeclNode(Obj symbolObject) 
            : base(symbolObject)
        { }


        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public void AddAttr(AssignNode assign)
        {
            Attributes.Add(assign);
        }
    }
}
