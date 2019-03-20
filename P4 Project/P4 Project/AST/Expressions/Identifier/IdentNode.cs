using P4_Project.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Identifier
{
    /// <summary>
    /// This node represents an identifier
    /// </summary>
    public abstract class IdentNode : ExprNode
    {

        /// <summary>
        /// The identifier is the string value of the identifier, and the reference is a reference directly to the symbol table.
        /// </summary>
        Reference reference;
        public string identifier;

        public IdentNode(string _identifier)
        {
            identifier = _identifier;
        }
    }
}
