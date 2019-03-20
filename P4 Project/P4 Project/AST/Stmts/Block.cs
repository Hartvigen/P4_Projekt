using P4_Project.AST;
using P4_Project.AST.Stmts;
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
    public class Block : StmtNode
    {
        public List<StmtNode> commands = new List<StmtNode>();


        public Block()
        { }


        public void Add(StmtNode com)
        {
            commands.Add(com);
        }
    }
}
