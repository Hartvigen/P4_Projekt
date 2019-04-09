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
    /// made with the syntax collec<type> col = {expr1, ..., exprn}, symbolizing the {expr1, ..., exprn} part.
    /// </summary>
    public class CollecConst : ExprNode
    {
        public List<ExprNode> Expressions { get; private set; } = new List<ExprNode>();


        public CollecConst(){ }


        public void Add(ExprNode expr)
        {
            Expressions.Add(expr);
        }

        public override object Accept(Visitor vi, object o)
        {
            vi.Visit(this, null);
            return null;
        }
    }
}
