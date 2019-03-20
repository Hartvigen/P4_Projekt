using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "DeclNode" is the common type for all declarations.
    /// </summary>
    public abstract class DeclNode : StmtNode
    {
        public string symbolName;

        public DeclNode() { }

        public DeclNode(string _symbolName)
        {
            symbolName = _symbolName;
        }
    }
}
