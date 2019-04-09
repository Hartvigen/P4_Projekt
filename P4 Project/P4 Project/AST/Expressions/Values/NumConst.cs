using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'number'.
    /// </summary>
    public class NumConst : ExprNode
    {
        public double Value { get; private set; }


        public NumConst() { }

        public NumConst(double value)
        {
            Value = value;
        }


        public override object Accept(Visitor vi, object o)
        {
            vi.Visit(this, null);
            return null;
        }

        public string GetString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
