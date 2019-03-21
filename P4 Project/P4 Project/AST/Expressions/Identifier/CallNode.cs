using P4_Project.AST.Expressions.Values;
using P4_Project.Visitors;
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
    public class CallNode : IdentNode
    {
        public CollecConst parameters;

        public CallNode()
            : base("")
        { }

        public CallNode(string _identifier, CollecConst _parameters) 
            : base(_identifier)
        {
            parameters = _parameters;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
