using System.Collections.Generic;
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
        public override List<string> getValue()
        {
            List<string> values = new List<string>();
            values.Add(Value ? "true" : "false");
            return values;
        }
    }
}
