using System;
using System.Collections.Generic;
using System.Reflection;
using P4_Project.AST;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Compiler.Interpreter
{
    public class Value
    {
        public readonly object o;

        public BaseType type;

        /// <summary>
        /// If the Object is not an empty collection this constructor can identify the correct type.
        /// </summary>
        /// <param name="v"></param>
        public Value(object v)
        {
            o = v;
            IdentifyType(v);
        }
        
        /// <summary>
        /// This Constructor is needed to collections that are empty as the type cannot be derived from objects within.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="type"></param>
        public Value(object v, BaseType type)
        {
            o = v;
            this.type = type;
        }
        
        private void IdentifyType(object v)
        {
            Type vType = v.GetType();
            switch (PreDefined.TypeMap[vType])
            {
                case 0: type = new BaseType("none"); break;
                case 1: type = new BaseType("boolean"); break;
                case 2: type = new BaseType(new BaseType("boolean"), new BaseType("list")); break;
                case 3: type = new BaseType(new BaseType("boolean"), new BaseType("set")); break;
                case 4: type = new BaseType(new BaseType("boolean"), new BaseType("stack")); break;
                case 5: type = new BaseType(new BaseType("boolean"), new BaseType("queue")); break;
                case 6: type = new BaseType("number"); break;
                case 7: type = new BaseType(new BaseType("number"), new BaseType("list")); break;
                case 8: type = new BaseType(new BaseType("number"), new BaseType("set")); break;
                case 9: type = new BaseType(new BaseType("number"), new BaseType("stack")); break;
                case 10: type = new BaseType(new BaseType("number"), new BaseType("queue")); break;
                case 11: type = new BaseType("text"); break;
                case 12: type = new BaseType(new BaseType("text"), new BaseType("list")); break;
                case 13: type = new BaseType(new BaseType("text"), new BaseType("set")); break;
                case 14: type = new BaseType(new BaseType("text"), new BaseType("stack")); break;
                case 15: type = new BaseType(new BaseType("text"), new BaseType("queue")); break;
                case 16: type = new BaseType("vertex"); break;
                case 17: type = new BaseType(new BaseType("vertex"), new BaseType("list")); break;
                case 18: type = new BaseType(new BaseType("vertex"), new BaseType("set")); break;
                case 19: type = new BaseType(new BaseType("vertex"), new BaseType("stack")); break;
                case 20: type = new BaseType(new BaseType("vertex"), new BaseType("queue")); break;
                case 21: type = new BaseType("edge"); break;
                case 22: type = new BaseType(new BaseType("edge"), new BaseType("list")); break;
                case 23: type = new BaseType(new BaseType("edge"), new BaseType("set")); break;
                case 24: type = new BaseType(new BaseType("edge"), new BaseType("stack")); break;
                case 25: type = new BaseType(new BaseType("edge"), new BaseType("queue")); break;
                default: throw new Exception(v.GetType() + " is not supported!");
            }
        }
        
    }
}
