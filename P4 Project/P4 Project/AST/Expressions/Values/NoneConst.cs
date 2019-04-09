using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As MAGIA uses none instead of null, this const represents the none constant.
    /// </summary>
    public class NoneConst : ExprNode
    {
        public override object Accept(Visitor vi, object o)
        {
            vi.Visit(this, null);
            return null;
        }
    }
}
