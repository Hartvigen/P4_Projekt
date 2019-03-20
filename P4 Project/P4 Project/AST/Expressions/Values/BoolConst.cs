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
        bool val;

        public BoolConst() { }

        public BoolConst(bool _val)
        {
            val = _val;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
