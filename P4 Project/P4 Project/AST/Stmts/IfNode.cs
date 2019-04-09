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
    /// The "IfNode" represents the structure of an if statement.
    /// </summary>
    public class IfNode : StmtNode
    {
        public ExprNode Condition { get; private set; }
        public BlockNode Body { get; private set; }
        public IfNode ElseNode { get; private set; }


        public IfNode() { }

        public IfNode(ExprNode condition, BlockNode body)
        {
            Condition = condition;
            Body = body;
        }


        public void SetElse(IfNode elseNode)
        {
            ElseNode = elseNode;
        }

        public override object Accept(Visitor vi, object o)
        {
            vi.Visit(this, null);
            return null;
        }
    }
}
