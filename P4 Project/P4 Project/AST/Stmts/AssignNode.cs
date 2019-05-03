using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "AssignNode" represents the structure of an assignment operation (giving some target identifier a value)
    /// </summary>
    public class AssignNode : StmtNode
    {
        public IdentNode Target { get; }
        public ExprNode Value { get; }
        public AssignNode(IdentNode target, ExprNode value)
        {
            Target = target;
            Value = value; 
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
