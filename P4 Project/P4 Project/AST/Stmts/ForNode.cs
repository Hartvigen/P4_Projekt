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
    /// The "ForNode" represents the structure of a for statement
    /// </summary>
    public class ForNode : StmtNode
    {
        public StmtNode Initializer { get; private set; }
        public ExprNode Condition { get; private set; }
        public StmtNode Iterator { get; private set; }

        public BlockNode Body { get; private set; }

        public ForNode(StmtNode initializer, ExprNode condition, StmtNode iterator, BlockNode body)
        {
            Initializer = initializer;
            Condition = condition;
            Iterator = iterator;
            Body = body;
        }

        public override object Accept(Visitor vi, object o)
        {

            vi.Visit(this, o);
            return null;
        }
    }
}
