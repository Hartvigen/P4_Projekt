﻿using P4_Project.AST.Expressions;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts
{
    /// <summary>
    /// The "IfNode" represents the structure of an if statement.
    /// </summary>
    public class IfNode : StmtNode
    {
        public ExprNode condition;
        public Block body;
        public IfNode elseNode = null;

        public IfNode() { }

        public IfNode(ExprNode e, Block b)
        {
            condition = e;
            body = b;
        }

        public void SetElse(IfNode i)
        {
            elseNode = i;
        }

        public override void Accept(Visitor vi)
        {
            vi.Visit(this);
        }
    }
}
