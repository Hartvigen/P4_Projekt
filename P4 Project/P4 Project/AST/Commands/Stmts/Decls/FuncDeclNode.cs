using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    class FuncDeclNode : DeclNode
    {
        List<VarDeclNode> funcParams = new List<VarDeclNode>();
        List<StmtNode> funcStmts = new List<StmtNode>();


        public FuncDeclNode(string _symbolName) : base(_symbolName)
        { }


        public void AddParam(VarDeclNode param)
        {
            funcParams.Add(param);
        }

        public void AddStmt(StmtNode stmt)
        {
            funcStmts.Add(stmt);
        }
    }
}
