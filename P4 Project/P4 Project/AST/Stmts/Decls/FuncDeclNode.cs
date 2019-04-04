using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.SymTab;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "FuncDeclNode" represents the declaration of a function.
    /// </summary>
    public class FuncDeclNode : DeclNode
    {
        public Obj FuncObj { get; private set; }
        public Block parameters;
        public Block body;

        public FuncDeclNode() { }

        public FuncDeclNode(string _symbolName, Obj _funcObj, Block _parameters, Block _body) : base(_symbolName)
        {
            FuncObj = _funcObj;
            parameters = _parameters;
            body = _body;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
