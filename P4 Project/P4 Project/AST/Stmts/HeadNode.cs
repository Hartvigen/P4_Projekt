using P4_Project.AST.Stmts.Decls;
using P4_Project.Visitors;
using System;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "HeadNode" represent the edge/vertex heads, in which non-standard attributes are declared
    /// </summary>
    public class HeadNode : StmtNode
    {
        public const int Vertex = 1, Edge = 2;
        private readonly int _type;
        public readonly BlockNode attrDeclBlock = new BlockNode();
        public HeadNode(int type)
        {
            _type = type;
        }
        public void AddAttr(VarDeclNode attrDecl)
        {
            attrDeclBlock.Add(attrDecl);
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
        public string GetName()
        {
            switch (_type)
            {
                case 1:
                    return "vertex";
                case 2:
                    return "edge";
                default:
                    throw new Exception("type: " + _type + " Is not supported");
            }
        }
    }
}
