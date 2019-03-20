using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'number'.
    /// </summary>
    class NumConst : ExprNode
    {
        double val;

        public NumConst(double _val)
        {
            val = _val;
        }

    }
}
