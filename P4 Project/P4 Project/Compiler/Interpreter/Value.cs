using System;
using System.Collections.Generic;
using P4_Project.AST;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Compiler.Interpreter
{
    public class Value
    {
        private static readonly Dictionary<Type, int> TypeMap = new Dictionary<Type, int>();

        public readonly object o;

        public BaseType type;

        public Value(object v)
        {
            o = v;
            if (TypeMap.Count == 0)
                SetKeys();
            SetObjectAndType(v);
        }

        private static void SetKeys()
        {
            TypeMap.Add(typeof(NoneConst), 0);
            TypeMap.Add(typeof(bool), 1);
            TypeMap.Add(typeof(List<bool>), 2);
            TypeMap.Add(typeof(HashSet<bool>), 3);
            TypeMap.Add(typeof(Stack<bool>), 4);
            TypeMap.Add(typeof(Queue<bool>), 5);
            TypeMap.Add(typeof(double), 6);
            TypeMap.Add(typeof(List<double>), 7);
            TypeMap.Add(typeof(HashSet<double>), 8);
            TypeMap.Add(typeof(Stack<double>), 9);
            TypeMap.Add(typeof(Queue<double>), 10);
            TypeMap.Add(typeof(string), 11);
            TypeMap.Add(typeof(List<string>), 12);
            TypeMap.Add(typeof(HashSet<string>), 13);
            TypeMap.Add(typeof(Stack<string>), 14);
            TypeMap.Add(typeof(Queue<string>), 15);
            TypeMap.Add(typeof(Vertex), 16);
            TypeMap.Add(typeof(List<Vertex>), 17);
            TypeMap.Add(typeof(HashSet<Vertex>), 18);
            TypeMap.Add(typeof(Stack<Vertex>), 19);
            TypeMap.Add(typeof(Queue<Vertex>), 20);
            TypeMap.Add(typeof(Edge), 21);
            TypeMap.Add(typeof(List<Edge>), 22);
            TypeMap.Add(typeof(HashSet<Edge>), 23);
            TypeMap.Add(typeof(Stack<Edge>), 24);
            TypeMap.Add(typeof(Queue<Edge>), 25);
            TypeMap.Add(typeof(List<object>), 26);
            TypeMap.Add(typeof(HashSet<object>), 27);
            TypeMap.Add(typeof(Stack<object>), 28);
            TypeMap.Add(typeof(Queue<object>), 29);

        }

        private void SetObjectAndType(object v)
        {
            switch (TypeMap[v.GetType()])
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
                case 26: IdentifyObject(v); break;
                case 27: IdentifyObject(v); break;
                case 28: IdentifyObject(v); break;
                case 29: IdentifyObject(v); break;
                default: throw new Exception(v.GetType() + " is not supported!");
            }
        }

        private void IdentifyObject(object v)
        {
            switch (v)
            {
                case List<object> l1:                   
                    switch (l1[0].GetType().Name)
                    {
                        case "Boolean" : type = new BaseType(new BaseType("boolean"), new BaseType("list")); break;
                        case "Double": type = new BaseType(new BaseType("number"), new BaseType("list")); break;
                        case "String": type = new BaseType(new BaseType("text"), new BaseType("list")); break;
                        case "Vertex": type = new BaseType(new BaseType("vertex"), new BaseType("list")); break;
                        case "Edge" : type = new BaseType(new BaseType("edge"), new BaseType("list")); break;
                        default: break;
                    }
                    break;
                case HashSet<object> l2:
                    switch (l2.GetEnumerator())
                    {
                        //case "Boolean": type = new BaseType(new BaseType("boolean"), new BaseType("list")); break;
                        //case "Double": type = new BaseType(new BaseType("number"), new BaseType("list")); break;
                        //case "String": type = new BaseType(new BaseType("text"), new BaseType("list")); break;
                        //case "Vertex": type = new BaseType(new BaseType("vertex"), new BaseType("list")); break;
                        //case "Edge": type = new BaseType(new BaseType("edge"), new BaseType("list")); break;
                        default: break;
                    }
                    break;
                case Stack<object> l3:
                    switch (l3.GetEnumerator().Current)
                    {
                        case bool _: type = new BaseType(new BaseType("stack"), new BaseType("boolean")); break;
                        case double _: type = new BaseType(new BaseType("stack"), new BaseType("number")); break;
                        case string _: type = new BaseType(new BaseType("stack"), new BaseType("text")); break;
                        case Vertex _: type = new BaseType(new BaseType("stack"), new BaseType("vertex")); break;
                        case Edge _: type = new BaseType(new BaseType("stack"), new BaseType("edge")); break;
                    }
                    break;
                case Queue<object> l4:
                    switch (l4.GetEnumerator().Current)
                    {
                        case bool _: type = new BaseType(new BaseType("queue"), new BaseType("boolean")); break;
                        case double _: type = new BaseType(new BaseType("queue"), new BaseType("number")); break;
                        case string _: type = new BaseType(new BaseType("queue"), new BaseType("text")); break;
                        case Vertex _: type = new BaseType(new BaseType("queue"), new BaseType("vertex")); break;
                        case Edge _: type = new BaseType(new BaseType("queue"), new BaseType("edge")); break;
                    }
                    break;
            }
        }
    }
}
