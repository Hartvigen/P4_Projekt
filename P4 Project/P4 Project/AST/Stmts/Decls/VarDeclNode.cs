using P4_Project.AST.Expressions;
using P4_Project.SymbolTable;
using P4_Project.Types;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "VarDeclNode" represents the declaration of a simple variable of any type.
    /// </summary>
    public class VarDeclNode : DeclNode
    {
        public ExprNode DefaultValue { get; }
        public readonly BaseType type;

        public VarDeclNode(Obj symbolObject, ExprNode defaultValue)
            : base(symbolObject)
        {
            DefaultValue = defaultValue;
            type = symbolObject.Type;
        }


        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }

        public string GetVarType()
        {
            return SymbolObject.Type.ToString();
        }
    }
}
