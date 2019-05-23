using System.Collections.Generic;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a collection of expressions,
    /// made with the syntax Collection&lt;Type&gt; col = {expr1, ..., exprn}, symbolizing the {expr1, ..., exprn} part.
    /// </summary>
    public class CollecConstNode : ExprNode
    {
        public List<ExprNode> Expressions { get; } = new List<ExprNode>();
        
        public CollecConstNode(BaseType type)
        {
            this.type = type;
        }


        public void Add(ExprNode expr)
        {
            Expressions.Add(expr);
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
