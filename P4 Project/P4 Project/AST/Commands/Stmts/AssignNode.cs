using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts
{
    class AssignNode : StmtNode
    {
        IdentNode target;
        ExprNode value;

        public AssignNode(IdentNode _target, ExprNode _value)
        {
            target = _target;
            value = _value; 
        }
    }
}
