using P4_Project.AST.Stmts.Decls;
using System;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "HeadNode" represent the edge/vertex heads, in which non-standard attributes are declared
    /// </summary>
    public class HeadNode : StmtNode
    {
        public const int Vertex = 1, Edge = 2;
        public readonly BaseType type;
        public readonly BlockNode attrDeclBlock = new BlockNode();
        public HeadNode(int type)
        {
            switch (type)
            {
                case Vertex:
                    this.type = new BaseType("vertex");
                    break;
                case Edge:
                    this.type = new BaseType("edge");
                    break;
                default:
                    throw new Exception("type: " + type + " is not a supported type, 1 = vertex, 2 = edge!");
            }
        }
        public void AddAttr(VarDeclNode attrDecl)
        {
            attrDeclBlock.Add(attrDecl);
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
