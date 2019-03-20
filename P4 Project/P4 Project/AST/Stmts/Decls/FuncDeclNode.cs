using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "FuncDeclNode" represents the declaration of a function.
    /// </summary>
    public class FuncDeclNode : DeclNode
    {
        public Block parameters;
        public Block body;

        public FuncDeclNode() { }

        public FuncDeclNode(string _symbolName, Block _parameters, Block _body) : base(_symbolName)
        {
            parameters = _parameters;
            body = _body;
        }
    }
}
