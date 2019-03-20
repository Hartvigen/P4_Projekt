using P4_Project.AST.Expressions.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    /// <summary>
    /// This node represents accessing fields and methods using the dot operator. 
    /// </summary>
    class MemberNode : IdentNode
    {
        /// <summary>
        /// memIdent is the name of the field or method that is being accessed, while source is the location of said field or method.
        /// </summary>
        public ExprNode source;
        public IdentNode memberIdent;

        public MemberNode(ExprNode _source, IdentNode _memberIdent) 
            : base(_memberIdent.identifier)
        {
            source = _source;
            memberIdent = _memberIdent;
        }
    }
}
