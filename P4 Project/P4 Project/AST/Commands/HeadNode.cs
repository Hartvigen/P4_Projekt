using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands
{
    class HeadNode : Command
    {
        public const int VERTEX = 1, EDGE = 2;

        public int type;

        public HeadNode(int _type)
        {
            type = _type;
        }
    }
}
