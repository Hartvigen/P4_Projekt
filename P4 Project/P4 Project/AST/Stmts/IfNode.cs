using P4_Project.AST.Expressions;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "IfNode" represents the structure of an if statement.
    /// </summary>
    public class IfNode : StmtNode
    {
        public ExprNode Condition { get; }
        public BlockNode Body { get; }
        public IfNode ElseNode { get; private set; }
        public IfNode(ExprNode condition, BlockNode body)
        {
            Condition = condition;
            Body = body;
        }
        public void SetElse(IfNode elseNode)
        {
            ElseNode = elseNode;
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
