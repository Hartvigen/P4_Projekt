using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'boolean'.
    /// </summary>
    public class BoolConstNode : ExprNode
    {
        public bool Value { get; }
        public BoolConstNode(bool value)
        {
            Value = value;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }
}
