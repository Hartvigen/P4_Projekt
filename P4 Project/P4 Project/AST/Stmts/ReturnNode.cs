using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Expressions;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "ReturnNode" represents a return statement.
    /// </summary>
    public class ReturnNode : StmtNode
    {
        ExprNode ret;

        public ReturnNode() { }

        public ReturnNode(ExprNode _ret)
        {
            ret = _ret;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
