using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    class VertexDeclNode : VEDeclNode
    {
        int type;

        public VertexDeclNode(int _type, string _symbolName) 
            : base(_symbolName)
        {
            type = _type;
        }
    }
}
