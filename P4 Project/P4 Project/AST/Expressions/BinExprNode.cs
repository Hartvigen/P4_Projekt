using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    class BinExprNode : ExprNode
    {
        ExprNode left, right;
        int operatorType;

        public BinExprNode(ExprNode _left, int _operatorType, ExprNode _right)
        {
            left = _left;
            right = _right;
            operatorType = _operatorType;
        }
    }
}
