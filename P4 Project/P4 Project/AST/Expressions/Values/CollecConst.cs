using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.Visitors;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a collection of expressions,
    /// made with the syntax collec<type> = {expr1, ..., exprn}, symbolizing the {expr1, ..., exprn} part.
    /// </summary>
    public class CollecConst : ExprNode
    {
        public List<ExprNode> exprs = new List<ExprNode>();

        public CollecConst(){ }

        public void Add(ExprNode expr)
        {
            exprs.Add(expr);
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
