using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    class VEDeclNode : DeclNode
    {
        Block b = new Block();
        int type;

        public VEDeclNode(int _type, string _symbolName) 
            : base(_symbolName)
        {
            type = _type;
        }

        public void AddAttr(AssignNode v)
        {
            b.Add(v);
        }
    }
}
