using P4_Project.AST.Stmts.Decls;
using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "ForeachNode" represents the structure of a foreach statement.
    /// </summary>
    class ForeachNode : StmtNode
    {
        VarDeclNode iterationVar;
        ExprNode iterator;

        Block body;

        public ForeachNode() { }

        public ForeachNode(VarDeclNode v, ExprNode e, Block b)
        {
            iterationVar = v;
            iterator = e;
            body = b;
        }
    }
}
