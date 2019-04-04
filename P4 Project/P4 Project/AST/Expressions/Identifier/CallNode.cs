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
        public CollecConst Parameters { get; private set; }


        public CallNode()
            : base(null)
        { }

        public CallNode(string identifier, CollecConst parameters) 
            : base(identifier)
        {
            Parameters = parameters;
        }


        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
