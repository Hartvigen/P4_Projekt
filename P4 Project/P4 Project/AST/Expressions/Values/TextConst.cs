﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    /// <summary>
    /// As the name suggests, this node represents a constant of the type 'text'.
    /// </summary>
    class TextConst : ExprNode
    {
        string value;

        public TextConst() { }

        public TextConst(string _value)
        {
            value = _value;
        }
    }
}
