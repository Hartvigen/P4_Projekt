using P4_Project.Compiler.SemanticAnalysis.Visitors;

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
        public BinExprNode(ExprNode left, int operatorType, ExprNode right)
        {
            Left = left;
            Right = right;
            OperatorType = operatorType;
        }
        public string GetCodeOfOperator()
        {
            return Operators.GetCodeFromInt(OperatorType);
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
