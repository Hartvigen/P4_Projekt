using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    public class BreakNode : StmtNode
    {
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
