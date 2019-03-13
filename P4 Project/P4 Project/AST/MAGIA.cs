using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Commands;

namespace P4_Project.AST
{
    public class MAGIA : Node
    {
        List<Command> commands = new List<Command>();


        public MAGIA()
        { }

        public void Add(Command com)
        {
            commands.Add(com);
        }
    }
}
