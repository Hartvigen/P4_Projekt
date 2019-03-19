using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Expressions;

namespace P4_Project.AST.Commands.Stmts
{
    class ReturnNode:StmtNode
    {
        ExprNode ret;

        public ReturnNode(ExprNode _ret)
        {
            ret = _ret;
        }

    }
}
