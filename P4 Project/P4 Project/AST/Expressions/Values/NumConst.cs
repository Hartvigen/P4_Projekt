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
        double value;

        public NumConst() { }

        public NumConst(double _value)
        {
            value = _value;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public string getString() {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
