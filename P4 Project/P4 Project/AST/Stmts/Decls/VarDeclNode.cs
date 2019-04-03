using P4_Project.AST.Expressions;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "VarDeclNode" represents the declaration of a simple variable of any type.
    /// </summary>
    public class VarDeclNode : DeclNode
    {
        public int type;
        public ExprNode expr; // If the variable should have a default value, this value is stored in "expr".

        public VarDeclNode() { }

        public VarDeclNode(int _type, string _symbolName, ExprNode _expr)
            : base(_symbolName)
        {
            type = _type;
            expr = _expr;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public String getVarType()
        {
            return TypeS.getCodeFromInt(type);
        }
    }
}
