using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// Since a function call is an expression it cannot be called as is. 
    /// Therefore "LoneCallNode" is a statement and container for a CallNode to allow calling a function as a statement
    /// </summary>
    public class LoneCallNode : StmtNode
    {
        public IdentNode call;
        
        
        public LoneCallNode(IdentNode call)
        {
            this.call = call;   
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
