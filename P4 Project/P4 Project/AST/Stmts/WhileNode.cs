using P4_Project.AST.Expressions;
using P4_Project.Visitors;
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
    public class WhileNode : StmtNode
    {
        public ExprNode condition;
        public Block body;

        public WhileNode(ExprNode _condition, Block _block)
        {
            condition = _condition;
            body = _block;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
