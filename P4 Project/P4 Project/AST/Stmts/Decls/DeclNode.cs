using P4_Project.SymbolTable;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "DeclNode" is the common type for all declarations.
    /// </summary>
    public abstract class DeclNode : StmtNode
    {
        public Obj SymbolObject { get; }
        protected DeclNode(Obj symbolObject)
        {
            SymbolObject = symbolObject;
        }
    }
}
