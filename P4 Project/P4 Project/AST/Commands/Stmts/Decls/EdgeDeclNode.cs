using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Expressions.Identifier;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    class EdgeDeclNode: VEDeclNode
    {
        private IdentNode start, end;


        private int Operator;

        public EdgeDeclNode(IdentNode _start, IdentNode _end, int _operator)
            :base(_start.identifier+_end.identifier)
        {
            start = _start;
            end = _end;
            Operator = _operator;
        }

    }
}
