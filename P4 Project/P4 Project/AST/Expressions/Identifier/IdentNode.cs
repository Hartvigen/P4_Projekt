using P4_Project.SymTab;
using P4_Project.Visitors;
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
        public string identifier;

        public IdentNode() { }

        public IdentNode(string _identifier)
        {
            identifier = _identifier;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
