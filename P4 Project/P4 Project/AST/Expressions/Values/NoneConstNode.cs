using P4_Project.AST.Expressions;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As MAGIA uses none instead of null, this const represents the none constant.
    /// </summary>
    public class NoneConstNode : ExprNode
    {
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public override string ToString()
        {
            return "none";
        }

        public override bool Equals(object obj)
        {
            return (obj is NoneConstNode);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
