using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "HeadNode" represent the edge/vertex heads, in which non-standard attributes are declared
    /// </summary>
    class HeadNode : StmtNode
    {
        public const int VERTEX = 1, EDGE = 2;

        public int type;
        public Block attrDeclBlock = new Block();


        public HeadNode(int _type)
        {
            type = _type;
        }

        public void AddAttr(VarDeclNode attrDecl)
        {
            attrDeclBlock.Add(attrDecl);
        }
    }
}
