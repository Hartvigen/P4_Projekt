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
        public ExprNode expr;
        int operatorType;

        public UnaExprNode() { }

        public UnaExprNode(int _operatorType, ExprNode _expr)
        {
            operatorType = _operatorType;
            expr = _expr;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public String getNameOfOperator()
        {
            return Operators.getNameFromInt(operatorType);
        }

        public String getCodeofOperator()
        {
            return Operators.getCodeFromInt(operatorType);
        }
    }
}
