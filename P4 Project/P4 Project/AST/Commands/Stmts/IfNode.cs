using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts
{
    class IfNode : StmtNode
    {
        ExprNode condition;
        Block stmtBody;
        IfNode elseNode = null;

        public IfNode(ExprNode e, Block b)
        {
            condition = e;
            stmtBody = b;
        }

        public void SetElse(IfNode i)
        {
            elseNode = i;
        }
    }
}
