using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    class UnaExprNode : ExprNode
    {
        public ExprNode expr;
        int operatorType;

        public UnaExprNode(int _operatorType, ExprNode _expr)
        {
            operatorType = _operatorType;
            expr = _expr;
        }
    }
}
