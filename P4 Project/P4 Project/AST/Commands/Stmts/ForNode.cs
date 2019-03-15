using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts
{
    class ForNode : StmtNode
    {
        StmtNode initializer;
        ExprNode condition;
        StmtNode iterator;

        Block block;

        public ForNode(StmtNode init, ExprNode con, StmtNode iter, Block b)
        {
            initializer = init;
            condition = con;
            iterator = iter;
            block = b;
        }
    }
}
