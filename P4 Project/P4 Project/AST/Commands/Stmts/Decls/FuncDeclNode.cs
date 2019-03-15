using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    public class FuncDeclNode : DeclNode
    {
        public Block paramBlock;
        public Block stmtBlock;


        public FuncDeclNode(string _symbolName, Block _paramBlock, Block _stmtBlock) : base(_symbolName)
        {
            paramBlock = _paramBlock;
            stmtBlock = _stmtBlock;
        }
    }
}
