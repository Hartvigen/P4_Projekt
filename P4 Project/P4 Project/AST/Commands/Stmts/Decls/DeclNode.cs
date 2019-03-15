using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    public class DeclNode : StmtNode
    {
        string symbolName;

        public DeclNode(string _symbolName)
        {
            symbolName = _symbolName;
        }

    }
}
