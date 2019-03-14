using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    class VarDeclNode : DeclNode
    {
        ExprNode expr;

        public VarDeclNode(int _type, string _symbolName, ExprNode _expr)
            : base(_type, _symbolName)
        {
            expr = _expr;
        }
    }
}
