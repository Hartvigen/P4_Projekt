using P4_Project.AST.Expressions;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "ForNode" represents the structure of a for statement
    /// </summary>
    public class ForNode : StmtNode
    {
        public StmtNode Initializer { get; }
        public ExprNode Condition { get; }
        public StmtNode Iterator { get; }
        public BlockNode Body { get; }
        public ForNode(StmtNode initializer, ExprNode condition, StmtNode iterator, BlockNode body)
        {
            Initializer = initializer;
            Condition = condition;
            Iterator = iterator;
            Body = body;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
