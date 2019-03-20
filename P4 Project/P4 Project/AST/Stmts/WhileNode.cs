using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "WhileNode" represents the structure of a while loop.
    /// </summary>
    class WhileNode : StmtNode
    {
        public ExprNode condition;
        public Block body;

        public WhileNode(ExprNode _condition, Block _block)
        {
            condition = _condition;
            body = _block;
        }
    }
}
