using P4_Project.Visitors;
using System.Collections.Generic;
using System.Globalization;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'number'.
    /// </summary>
    public class NumConst : ExprNode
    {
        public double Value { get; }
        public NumConst(double value)
        {
            Value = value;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
        public string GetString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
