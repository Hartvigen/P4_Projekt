using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Stmts
{
    public class ContinueNode : StmtNode
    {
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
