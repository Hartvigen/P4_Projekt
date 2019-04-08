using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "HeadNode" represent the edge/vertex heads, in which non-standard attributes are declared
    /// </summary>
    public class HeadNode : StmtNode
    {
        public const int VERTEX = 1, EDGE = 2;

        public int type;
        public BlockNode attrDeclBlock = new BlockNode();


        public HeadNode() { }

        public HeadNode(int _type)
        {
            type = _type;
        }


        public void AddAttr(VarDeclNode attrDecl)
        {
            attrDeclBlock.Add(attrDecl);
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public string getName()
        {
            if (type == 1)
                return "vertex";
            if (type == 2)
                return "edge";

            throw new Exception("type: " + type + " Is not supported");
        }
    }
}
