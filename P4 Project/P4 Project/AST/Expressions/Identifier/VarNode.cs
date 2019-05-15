using System.Collections.Generic;
using P4_Project.Compiler.SemanticAnalysis.Visitors;

namespace P4_Project.AST.Expressions.Identifier
{
    /// <summary>
    /// This is a specialization of an IdentNode, where the identifier represents a variable.
    /// </summary>
    public class VarNode : IdentNode
    {
        public VarNode(string ident)
            : base(ident)
        { }
        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
