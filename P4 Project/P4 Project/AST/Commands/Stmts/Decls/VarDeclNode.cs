using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    public class VarDeclNode : DeclNode
    {
        public int type;
        public ExprNode expr;

        public VarDeclNode(int _type, string _symbolName, ExprNode _expr)
            : base(_symbolName)
        {
            type = _type;
            expr = _expr;
        }
    }
}
