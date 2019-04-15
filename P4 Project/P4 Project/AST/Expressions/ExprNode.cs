using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.Types;
using P4_Project.Visitors;

namespace P4_Project.AST.Expressions
{
    /// <summary>
    /// An Expression is a specialization of node, used to getting all expressions on their own, without risking getting any statements. 
    /// </summary>
    public abstract class ExprNode : Node
    {
        public bool InParentheses { get; set; } = false;
        public BaseType type;
        public ExprNode()
        {
            type = null;
        }
    }
}
