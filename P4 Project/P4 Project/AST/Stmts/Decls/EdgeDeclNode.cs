using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "EdgeDeclNode" represents the declaration of an edge between two vertices
    /// </summary>
    public class EdgeDeclNode : VEDeclNode
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

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
