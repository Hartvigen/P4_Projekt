using System.Collections.Generic;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// A block is a list of Statements. That includes headers and function declarations, but is usually just the basic statements.
    /// For the most part, it serves as a collection of lines of code, where each line is an entry in the list.
    /// </summary>
    public class BlockNode : StmtNode
    {
        public List<StmtNode> Statements { get; } = new List<StmtNode>();
        public void Add(StmtNode com)
        {
            Statements.Add(com);
        }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
