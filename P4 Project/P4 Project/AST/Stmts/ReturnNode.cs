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
        public ExprNode Ret { get; private set; }


        public ReturnNode() { }

        public ReturnNode(ExprNode ret)
        {
            Ret = ret;
        }


        public override object Accept(Visitor vi, object o)
        {
            return vi.Visit(this, o);
        }
    }
}
