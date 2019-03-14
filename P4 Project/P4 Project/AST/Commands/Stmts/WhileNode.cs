using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts
{
    class WhileNode : StmtNode
    {
        ExprNode condition;
        Block stmts;

        public WhileNode(ExprNode e, Block b)
        {
            condition = e;
            stmts = b;
        }
    }
}
