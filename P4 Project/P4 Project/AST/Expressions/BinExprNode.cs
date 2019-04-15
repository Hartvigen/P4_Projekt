﻿using P4_Project.Types;
using P4_Project.Visitors;
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
    public class BinExprNode : ExprNode
    {
        public ExprNode Left { get; private set; }
        public ExprNode Right { get; private set; }
        public int OperatorType { get; private set; }
        public BaseType type { get;  set; }

        public BinExprNode() { }

        public BinExprNode(ExprNode left, int operatorType, ExprNode right)
        {
            Left = left;
            Right = right;
            OperatorType = operatorType;
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
