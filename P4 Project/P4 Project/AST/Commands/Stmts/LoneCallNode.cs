using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Expressions.Identifier;

namespace P4_Project.AST.Commands.Stmts
{
    class LoneCallNode : StmtNode
    {
        private IdentNode call;

        public LoneCallNode(IdentNode call)
        {
            this.call = call;
        }
    }
}
