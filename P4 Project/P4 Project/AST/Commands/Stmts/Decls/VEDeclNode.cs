using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Commands.Stmts.Decls
{
    abstract class VEDeclNode : DeclNode
    {
        Block attributes = new Block();

        protected VEDeclNode(string _symbolName) : base(_symbolName)
        {
        }
    }
}