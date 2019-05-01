using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    public class BreakNode : StmtNode
    {
        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }
    }
}
