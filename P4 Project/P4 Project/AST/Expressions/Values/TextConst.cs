using P4_Project.Visitors;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'text'.
    /// </summary>
    public class TextConst : ExprNode
    {
        public string Value { get; }
        
        public TextConst(string value)
        {
            Value = value;
        }
        
        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }
    }
}
