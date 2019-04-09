using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Stmts;
using P4_Project.Visitors;

namespace P4_Project.AST
{
    /// <summary>
    /// This node represents the top of the AST. It is this node which will be referenced when the visitor starts up.
    /// </summary>
    public class MAGIA : Node
    {
        /// <summary>
        /// This Block contains the entire list of statements that serve as the first layer of branches.
        /// That is, the headers, the statements of the program's body, and the function declarations.
        /// </summary>
        public BlockNode block;

        public MAGIA() { }

        public MAGIA(BlockNode _block)
        {
            block = _block;
        }

        public override object Accept(Visitor vi, object o)
        {
            vi.Visit(this, null);
            return null;
        }
    }
}
