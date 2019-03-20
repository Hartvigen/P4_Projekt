using P4_Project.AST.Expressions.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Identifier
{
    /// <summary>
    /// This node represents a specialization of IdentNode representing a call to a function or method.
    /// </summary>
    class CallNode : IdentNode
    {
        CollecConst collec;

        public CallNode(string _identifier, CollecConst _collec) 
            : base(_identifier)
        {
            collec = _collec;
        }
    }
}
