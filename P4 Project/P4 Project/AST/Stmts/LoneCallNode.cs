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
        public IdentNode Call { get; private set; }


        public LoneCallNode(IdentNode call)
        {
            Call = call;   
        }


        public override object Accept(Visitor vi, object o)
        {
            return vi.Visit(this, o);
        }
    }
}
