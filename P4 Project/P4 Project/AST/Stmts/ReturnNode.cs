using P4_Project.AST.Expressions;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "ReturnNode" represents a return statement.
    /// </summary>
    public class ReturnNode : StmtNode
    {
        public ExprNode Ret { get; }
        public ReturnNode(ExprNode ret)
        {
            Ret = ret;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
