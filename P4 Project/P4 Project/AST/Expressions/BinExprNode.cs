using System.Collections.Generic;
using P4_Project.Types;
using P4_Project.Visitors;

namespace P4_Project.AST.Expressions
{
    /// <summary>
    /// An expression where the expression contains two expressions(which might be factors) and an operator between them. 
    /// </summary>
    public class BinExprNode : ExprNode
    {
        public ExprNode Left { get; }
        public ExprNode Right { get; }
        public int OperatorType { get; }
        public BaseType Type { get;  set; }

        public BinExprNode(ExprNode left, int operatorType, ExprNode right)
        {
            Left = left;
            Right = right;
            OperatorType = operatorType;
        }


        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }

        public string GetNameOfOperator()
        {
            return Operators.getNameFromInt(OperatorType);
        }

        public string GetCodeOfOperator()
        {
            return Operators.getCodeFromInt(OperatorType);
        }
        
        public List<BaseType> GetOperandTypeOfOperator()
        {
            return Operators.getOperandTypeFronInt(OperatorType);
        }
    }
}
