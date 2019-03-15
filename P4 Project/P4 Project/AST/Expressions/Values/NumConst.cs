﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Expressions.Values
{
    class NumConst : ExprNode
    {
        int val;

        public NumConst(int _val)
        {
            val = _val;
        }
    }
}
