using P4_Project.AST.Stmts;
using P4_Project.Visitors;

namespace P4_Project.AST
{
    /// <summary>
    /// This node represents the top of the AST. It is this node which will be referenced when the visitor starts up.
    /// </summary>
    public class Magia : Node
    {
        /// <summary>
        /// This Block contains the entire list of statements that serve as the first layer of branches.
        /// That is, the headers, the statements of the program's body, and the function declarations.
        /// </summary>
        public readonly BlockNode block;

        public Magia(BlockNode block)
        {
            this.block = block;
        }

        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }
    }
}
