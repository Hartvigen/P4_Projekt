using P4_Project.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Identifier
{
    public abstract class IdentNode : ExprNode
    {
        Reference reference;
        public string identifier;

        public IdentNode(string _identifier)
        {
            identifier = _identifier;
        }
    }
}
