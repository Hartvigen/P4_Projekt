using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'text'.
    /// </summary>
    public class TextConst : ExprNode
    {
        public string Value { get; private set; }


        public TextConst() { }

        public TextConst(string value)
        {
            Value = value;
        }


        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
