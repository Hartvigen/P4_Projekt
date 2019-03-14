using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    class CollecConst
    {
        List<ExprNode> exprs = new List<ExprNode>();

        public CollecConst()
        { }

        public void Add(ExprNode expr)
        {
            exprs.Add(expr);
        }
    }
}
