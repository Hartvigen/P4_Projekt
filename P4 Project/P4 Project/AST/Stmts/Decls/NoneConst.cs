using P4_Project.AST.Expressions;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// As MAGIA uses none instead of null, this const represents the none constant.
    /// </summary>
    public class NoneConst : ExprNode
    {
        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }
    }
}
