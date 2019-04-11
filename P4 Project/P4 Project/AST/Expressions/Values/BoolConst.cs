using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'boolean'.
    /// </summary>
    public class BoolConst : ExprNode
    {
        public bool Value { get; private set; }


        public BoolConst() { }

        public BoolConst(bool value)
        {
            Value = value;
        }


        public override object Accept(Visitor vi, object o)
        {
            return vi.Visit(this, o);
        }

        public string GetString() {
            return Value ? "true" : "false";
        }
    }
}
