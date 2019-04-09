using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.SymTab;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "DeclNode" is the common type for all declarations.
    /// </summary>
    public abstract class DeclNode : StmtNode
    {
        public Obj SymbolObject { get; private set; }


        public DeclNode() { }

        public DeclNode(Obj symbolObject)
        {
            SymbolObject = symbolObject;
        }
    }
}
