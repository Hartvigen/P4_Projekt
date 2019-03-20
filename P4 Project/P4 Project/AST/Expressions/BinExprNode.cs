using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    /// <summary>
    /// An expression where the expression contains two expressions(which might be factors) and an operator between them. 
    /// </summary>
    class BinExprNode : ExprNode
    {
        ExprNode left, right;
        int operatorType;

        public BinExprNode() { }

        public BinExprNode(ExprNode _left, int _operatorType, ExprNode _right)
        {
            left = _left;
            right = _right;
            operatorType = _operatorType;
        }
    }
}
