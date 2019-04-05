﻿using System;
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
        public IdentNode Start { get; private set; }
        public int Operator { get; private set; }

        public List<Tuple<IdentNode, List<AssignNode>>> RightSides { get; private set; } = new List<Tuple<IdentNode, List<AssignNode>>>();
        

        public EdgeCreateNode(IdentNode start, int @operator)
        {
            Start = start;
            Operator = @operator;
        }


        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }

        public void AddRightSide(IdentNode rightVertex, List<AssignNode> attributes)
        {
            RightSides.Add(
                new Tuple<IdentNode, List<AssignNode>>(rightVertex, attributes)
            );
        }

        public string GetNameOfOperator()
        {
            return Operators.getNameFromInt(Operator);
        }

        public string GetCodeofOperator()
        {
            return Operators.getCodeFromInt(Operator);
        }
    }
}
