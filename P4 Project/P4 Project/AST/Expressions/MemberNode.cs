using P4_Project.AST.Expressions.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    class MemberNode : ExprNode
    {
        ExprNode source;
        IdentNode memberIdent;

        public MemberNode(ExprNode _source, IdentNode _memberIdent)
        {
            source = _source;
            memberIdent = _memberIdent;
        }
    }
}
