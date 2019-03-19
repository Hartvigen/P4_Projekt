using P4_Project.AST.Commands;
using P4_Project.AST.Commands.Stmts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST
{
    public class Block : StmtNode
    {
        public List<CommandNode> commands = new List<CommandNode>();


        public Block()
        { }


        public void Add(CommandNode com)
        {
            commands.Add(com);
        }
    }
}
