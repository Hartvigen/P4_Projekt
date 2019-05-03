using P4_Project.Visitors;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'boolean'.
    /// </summary>
    public class BoolConst : ExprNode
    {
        private bool Value { get; }
        public BoolConst(bool value)
        {
            Value = value;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
        public string GetString() {
            return Value ? "true" : "false";
        }
    }
}
