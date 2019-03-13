using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    class VarDecl : Decl
    {
        public VarDecl(int _type, string _symbolName) // Add parameter for the assigning expression 
            : base(_type, _symbolName)
        {
        }
    }
}
