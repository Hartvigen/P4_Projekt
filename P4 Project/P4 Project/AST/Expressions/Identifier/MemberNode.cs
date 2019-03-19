using P4_Project.AST.Expressions.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    class MemberNode : IdentNode
    {
        ExprNode source;
        IdentNode memberIdent;

        public MemberNode(ExprNode _source, IdentNode _memberIdent) 
            : base(_memberIdent.identifier)
        {
            source = _source;
            memberIdent = _memberIdent;
        }
    }
}
