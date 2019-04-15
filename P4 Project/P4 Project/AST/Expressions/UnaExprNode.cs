using P4_Project.Types;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    /// <summary>
    /// A unary expression is used for an expression where a boolean is either negated by the '!' operator or a number is made negative by a pre-fix '-'
    /// </summary>
    public class UnaExprNode : ExprNode
    {
        public ExprNode Expr { get; private set; }
        public int OperatorType { get; private set; }

        public BaseType type { get; set; }

        public UnaExprNode() { }

        public UnaExprNode(int operatorType, ExprNode expr)
        {
            OperatorType = operatorType;
            Expr = expr;
        }


        public override object Accept(Visitor vi, object o)
        {
            return vi.Visit(this, o);
        }

        public string GetNameOfOperator()
        {
            return Operators.getNameFromInt(OperatorType);
        }

        public string GetCodeofOperator()
        {
            return Operators.getCodeFromInt(OperatorType);
        }
    }
}
