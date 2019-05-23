using System.Globalization;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'number'.
    /// </summary>
    public class NumConstNode : ExprNode
    {
        public double Value { get; }
        public NumConstNode(double value)
        {
            Value = value;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
        public string GetString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
