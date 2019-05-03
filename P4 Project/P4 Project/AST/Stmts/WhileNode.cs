using P4_Project.AST.Expressions;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "WhileNode" represents the structure of a while loop.
    /// </summary>
    public class WhileNode : StmtNode
    {
        public ExprNode Condition { get; }
        public BlockNode Body { get; }
        public WhileNode(ExprNode condition, BlockNode block)
        {
            Condition = condition;
            Body = block;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
