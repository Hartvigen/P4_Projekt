using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    class Decl : Stmt
    {
        int type;
        string symbolName;

        public Decl(int _type, string _symbolName)
        {
            type = _type;
            symbolName = _symbolName;
        }

    }
}
