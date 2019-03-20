using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "ForNode" represents the structure of a for statement
    /// </summary>
    class ForNode : StmtNode
    {
        public StmtNode initializer;
        public ExprNode condition;
        public StmtNode iterator;

        public Block body;

        public ForNode(StmtNode init, ExprNode con, StmtNode iter, Block _body)
        {
            initializer = init;
            condition = con;
            iterator = iter;
            body = _body;
        }
    }
}
