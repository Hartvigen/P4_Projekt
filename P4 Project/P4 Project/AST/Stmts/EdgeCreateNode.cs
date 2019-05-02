using System;
using System.Collections.Generic;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "EdgeDeclNode" represents the declaration of an edge between two vertices
    /// </summary>
    public class EdgeCreateNode : StmtNode
    {
        public IdentNode LeftSide { get; }
        public int Operator { get; }

        public List<Tuple<IdentNode, List<AssignNode>>> RightSide { get; } = new List<Tuple<IdentNode, List<AssignNode>>>();
        

        public EdgeCreateNode(IdentNode start, int @operator)
        {
            LeftSide = start;
            Operator = @operator;
        }


        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }

        public void AddRightSide(IdentNode rightVertex, List<AssignNode> attributes)
        {
            RightSide.Add(
                new Tuple<IdentNode, List<AssignNode>>(rightVertex, attributes)
            );
        }

        public string GetNameOfOperator()
        {
            return Operators.GetNameFromInt(Operator);
        }

        public string GetCodeOfOperator()
        {
            return Operators.GetCodeFromInt(Operator);
        }
    }
}
