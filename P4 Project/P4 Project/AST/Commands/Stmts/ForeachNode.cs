using P4_Project.AST.Commands.Stmts.Decls;
using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts
{
    class ForeachNode : StmtNode
    {
        VarDeclNode var;
        ExprNode iterator;

        Block stmts;

        public ForeachNode(VarDeclNode v, ExprNode e, Block b)
        {
            var = v;
            iterator = e;
            stmts = b;
        }
    }
}
