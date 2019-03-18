using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Identifier
{
    class VarNode : IdentNode
    {
        public VarNode(string _identifier) 
            : base(_identifier)
        { }
    }
}
