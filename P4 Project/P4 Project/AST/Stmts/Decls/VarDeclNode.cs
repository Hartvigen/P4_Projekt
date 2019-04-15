using P4_Project.AST.Expressions;
using P4_Project.Types;
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
    /// The "VarDeclNode" represents the declaration of a simple variable of any type.
    /// </summary>
    public class VarDeclNode : DeclNode
    {
        public ExprNode DefaultValue { get; private set; }
        public BaseType Type;

        public VarDeclNode() { }

        public VarDeclNode(Obj symbolObject, ExprNode defaultValue)
            : base(symbolObject)
        {
            DefaultValue = defaultValue;
            Type = symbolObject.Type;
        }


        public override object Accept(Visitor vi, object o)
        {
            return vi.Visit(this, o);
        }

        public string GetVarType()
        {
            return SymbolObject.Type.ToString();
        }
    }
}
