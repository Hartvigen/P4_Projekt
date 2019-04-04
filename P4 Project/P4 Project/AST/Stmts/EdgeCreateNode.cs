using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "EdgeDeclNode" represents the declaration of an edge between two vertices
    /// </summary>
    public class EdgeCreateNode : StmtNode
    {
        public BlockNode Attributes { get; private set; } = new BlockNode();

        public IdentNode Start { get; private set; }
        public IdentNode End { get; private set; }

        public int Operator { get; private set; }


        public EdgeCreateNode(IdentNode start, IdentNode end, int @operator)
        {
            Start = start;
            End = end;
            Operator = @operator;
        }


        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public string GetNameOfOperator()
        {
            return Operators.getNameFromInt(Operator);
        }

        public string GetCodeofOperator()
        {
            return Operators.getCodeFromInt(Operator);
        }

        public void AddAttr(AssignNode assign)
        {
            Attributes.Add(assign);
        }
    }
}
