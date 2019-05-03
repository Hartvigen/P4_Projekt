using P4_Project.AST.Expressions.Values;
using P4_Project.Visitors;

namespace P4_Project.AST.Expressions.Identifier
{
    /// <summary>
    /// This node represents a specialization of IdentNode representing a call to a function or method.
    /// </summary>
    public class CallNode : IdentNode
    {
        public CollecConst Parameters { get; }
        public CallNode(string ident, CollecConst parameters) 
            : base(ident)
        {
            Parameters = parameters;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
