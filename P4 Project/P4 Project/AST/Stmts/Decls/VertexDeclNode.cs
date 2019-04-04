using P4_Project.AST.Expressions;
using P4_Project.Types;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "VertexDeclNode" represents the declaration of a vertex.
    /// </summary>
    public class VertexDeclNode : VEDeclNode
    {
        BaseType type;

        public VertexDeclNode() { }


        public VertexDeclNode(BaseType _type, string _symbolName) 
            : base(_symbolName)
        {
            type = _type;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
