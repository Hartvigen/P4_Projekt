using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    public class ContinueNode : StmtNode
    {
        public ContinueNode() { 
        
        }

        public override object Accept(Visitor vi, object o)
        {
            return vi.Visit(this, o);
        }
    }
}
