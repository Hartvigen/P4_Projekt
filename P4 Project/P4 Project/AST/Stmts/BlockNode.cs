using P4_Project.AST;
using P4_Project.AST.Stmts;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// A block is a list of Statements. That includes headers and function declarations, but is usually just the basic statements.
    /// For the most part, it serves as a collection of lines of code, where each line is an entry in the list.
    /// </summary>
    public class BlockNode : StmtNode
    {
        public List<StmtNode> statements = new List<StmtNode>();

        public BlockNode() { }

        public void Add(StmtNode com)
        {
            statements.Add(com);
        }

        public override object Accept(Visitor vi, object o)
        {

            vi.Visit(this, o);
            return null;
        }
    }
}
