using P4_Project.AST.Stmts.Decls;
using P4_Project.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "ForeachNode" represents the structure of a foreach statement.
    /// </summary>
    public class ForeachNode : StmtNode
    {
        public VarDeclNode IterationVar { get; private set; }
        public ExprNode Iterator { get; private set; }

        public BlockNode Body { get; private set; }


        public ForeachNode() { }

        public ForeachNode(VarDeclNode iterationVar, ExprNode iterator, BlockNode body)
        {
            IterationVar = iterationVar;
            Iterator = iterator;
            Body = body;
        }


        public override object Accept(Visitor vi, object o)
        {

            return vi.Visit(this, o);
        }
    }
}
