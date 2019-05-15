using P4_Project.Compiler.SemanticAnalysis.Visitors;
using P4_Project.SymbolTable;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "VertexDeclNode" represents the declaration of a vertex.
    /// </summary>
    public class VertexDeclNode : DeclNode
    {
        public BlockNode Attributes { get; } = new BlockNode();
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
