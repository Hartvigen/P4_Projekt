﻿using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Expressions
{
    /// <summary> 
    /// A unary expression is used for an expression where a boolean is either negated by the '!' operator or a number is made negative by a pre-fix '-' 
    /// </summary> 
    public class UnaExprNode : ExprNode
    {
        public ExprNode Expr { get; }
        public int OperatorType { get; }
        public BaseType Type { get; set; }
        public UnaExprNode(int operatorType, ExprNode expr)
        {
            OperatorType = operatorType;
            Expr = expr;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
