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
        IdentNode target;
        ExprNode value;

        public AssignNode() { }

        public AssignNode(IdentNode _target, ExprNode _value)
        {
            target = _target;
            value = _value; 
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
