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
        public ExprNode Condition { get; private set; }
        public BlockNode Body { get; private set; }


        public WhileNode(ExprNode condition, BlockNode block)
        {
            Condition = condition;
            Body = block;
        }


        public override object Accept(Visitor vi, object o)
        {

            return vi.Visit(this, o);
        }



    }
}
