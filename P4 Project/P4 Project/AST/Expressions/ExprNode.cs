using System.Collections.Generic;
using P4_Project.AST;

namespace P4_Project.AST.Expressions
{
    /// <summary>
    /// An Expression is a specialization of node, used to getting all expressions on their own, without risking getting any statements. 
    /// </summary>
    public abstract class ExprNode : Node
    {
        public bool InParentheses { get; set; }
        public BaseType type;
        
        protected ExprNode()
        {
            type = null;
        }

        public abstract List<string> getValue();
    }
}
