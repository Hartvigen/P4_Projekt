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
        public CollecConst parameters;

        public CallNode(string _identifier, CollecConst _parameters) 
            : base(_identifier)
        {
            parameters = _parameters;
        }
    }
}
