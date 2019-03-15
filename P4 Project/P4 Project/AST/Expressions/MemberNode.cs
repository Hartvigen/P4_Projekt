using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions
{
    class MemberNode : Node
    {
        string name;
        ExprNode source;

        public MemberNode(string _name, ExprNode _source)
        {
            name = _name;
            source = _source;
        }
    }
}
