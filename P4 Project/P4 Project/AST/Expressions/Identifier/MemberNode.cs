using P4_Project.AST.Expressions.Identifier;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Identifier
{
    /// <summary>
    /// This node represents accessing fields and methods using the dot operator. 
    /// </summary>
    public class MemberNode : IdentNode
    {
        /// <summary>
        /// memIdent is the name of the field or method that is being accessed, while source is the location of said field or method.
        /// </summary>
        public ExprNode Source { get; private set; }
        public IdentNode MemberIdent { get; private set; }


        public MemberNode() { }

        public MemberNode(ExprNode source, IdentNode memberIdent) 
            : base(memberIdent.Identifier)
        {
            Source = source;
            MemberIdent = memberIdent;
        }


        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
