using P4_Project.AST.Stmts.Decls;
using P4_Project.AST.Expressions;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "ForeachNode" represents the structure of a foreach statement.
    /// </summary>
    public class ForeachNode : StmtNode
    {
        public VarDeclNode IterationVar { get; }
        public ExprNode Iterator { get; }
        public BlockNode Body { get; }
        public ForeachNode(VarDeclNode iterationVar, ExprNode iterator, BlockNode body)
        {
            IterationVar = iterationVar;
            Iterator = iterator;
            Body = body;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
