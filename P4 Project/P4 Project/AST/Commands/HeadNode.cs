using P4_Project.AST.Commands.Stmts.Decls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands
{
    class HeadNode : CommandNode
    {
        public const int VERTEX = 1, EDGE = 2;

        public int type;
        public List<VarDeclNode> attrDecls = new List<VarDeclNode>();


        public HeadNode(int _type)
        {
            type = _type;
        }

        public void AddAttr(VarDeclNode attrDecl)
        {
            attrDecls.Add(attrDecl);
        }
    }
}
