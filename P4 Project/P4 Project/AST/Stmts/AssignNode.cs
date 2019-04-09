using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "AssignNode" represents the structure of an assignment operation (giving some target identifier a value)
    /// </summary>
    public class AssignNode : StmtNode
    {
        public IdentNode Target { get; private set; }
        public ExprNode Value { get; private set; }


        public AssignNode() { }

        public AssignNode(IdentNode target, ExprNode value)
        {
            Target = target;
            Value = value; 
        }


        public override object Accept(Visitor vi, object o)
        {

            vi.Visit(this, o);
            return null;
        }
    }
}
