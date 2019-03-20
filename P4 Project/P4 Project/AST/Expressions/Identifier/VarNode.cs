using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Identifier
{
    /// <summary>
    /// This is a specialization of an IdentNode, where the identifier represents a variable.
    /// </summary>
    class VarNode : IdentNode
    {
        public VarNode() { }

        public VarNode(string _identifier) 
            : base(_identifier)
        { }
    }
}
