using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a collection of expressions,
    /// made with the syntax collec<type> = {expr1, ..., exprn}, symbolizing the {expr1, ..., exprn} part.
    /// </summary>
    class CollecConst : ExprNode
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
