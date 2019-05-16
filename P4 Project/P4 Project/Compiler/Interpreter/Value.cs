using System;
using System.Collections;
using System.Collections.Generic;
using P4_Project.AST;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Compiler.Interpreter
{
    public class Value
    {
        private static Dictionary<Type, int> typeMap = new Dictionary<Type, int>();

        public object o;

        NoneConst none;

        bool boolean;
        List<bool> booleanList;
        HashSet<bool> booleanSet;
        Stack<bool> booleanStack;
        Queue<bool> booleanQueue;
        double number;
        List<double> numberList;
        HashSet<double> numberSet;
        Stack<double> numberStack;
        Queue<double> numberQueue;
        string text;
        List<string> textList;
        HashSet<string> textSet;
        Stack<string> textStack;
        Queue<string> textQueue;
        Vertex vertex;
        List<Vertex> vertexList;
        HashSet<Vertex> vertexSet;
        Stack<Vertex> vertexStack;
        Queue<Vertex> vertexQueue;
        Edge edge;
        List<Edge> edgeList;
        HashSet<Edge> edgeSet;
        Stack<Edge> edgeStack;
        Queue<Edge> edgeQueue;

        public BaseType type;

        public Value(object v)
        {
            o = v;
            if (typeMap.Count == 0)
                SetKeys();
            setObjectAndType(v);
        }

        private void SetKeys()
        {
            typeMap.Add(typeof(NoneConst), 0);
            typeMap.Add(typeof(bool), 1);
            typeMap.Add(typeof(List<bool>), 2);
            typeMap.Add(typeof(HashSet<bool>), 3);
            typeMap.Add(typeof(Stack<bool>), 4);
            typeMap.Add(typeof(Queue<bool>), 5);
            typeMap.Add(typeof(double), 6);
            typeMap.Add(typeof(List<double>), 7);
            typeMap.Add(typeof(HashSet<double>), 8);
            typeMap.Add(typeof(Stack<double>), 9);
            typeMap.Add(typeof(Queue<double>), 10);
            typeMap.Add(typeof(string), 11);
            typeMap.Add(typeof(List<string>), 12);
            typeMap.Add(typeof(HashSet<string>), 13);
            typeMap.Add(typeof(Stack<string>), 14);
            typeMap.Add(typeof(Queue<string>), 15);
            typeMap.Add(typeof(Vertex), 16);
            typeMap.Add(typeof(List<Vertex>), 17);
            typeMap.Add(typeof(HashSet<Vertex>), 18);
            typeMap.Add(typeof(Stack<Vertex>), 19);
            typeMap.Add(typeof(Queue<Vertex>), 20);
            typeMap.Add(typeof(Edge), 21);
            typeMap.Add(typeof(List<Edge>), 22);
            typeMap.Add(typeof(HashSet<Edge>), 23);
            typeMap.Add(typeof(Stack<Edge>), 24);
            typeMap.Add(typeof(Queue<Edge>), 25);
            typeMap.Add(typeof(List<object>), 26);
            typeMap.Add(typeof(HashSet<object>), 27);
            typeMap.Add(typeof(Stack<object>), 28);
            typeMap.Add(typeof(Queue<object>), 29);

        }

        private void setObjectAndType(object v)
        {
            switch (typeMap[v.GetType()])
            {
                case 0: none = (NoneConst)v; type = new BaseType("none"); break;
                case 1: boolean = (bool)v; type = new BaseType("boolean"); break;
                case 2: booleanList = (List<bool>)v; type = new BaseType(new BaseType("boolean"), new BaseType("list")); break;
                case 3: booleanSet = (HashSet<bool>)v; type = new BaseType(new BaseType("boolean"), new BaseType("set")); break;
                case 4: booleanStack = (Stack<bool>)v; type = new BaseType(new BaseType("boolean"), new BaseType("stack")); break;
                case 5: booleanQueue = (Queue<bool>)v; type = new BaseType(new BaseType("boolean"), new BaseType("queue")); break;
                case 6: number = (double)v; type = new BaseType("number"); break;
                case 7: numberList = (List<double>)v; type = new BaseType(new BaseType("number"), new BaseType("list")); break;
                case 8: numberSet = (HashSet<double>)v; type = new BaseType(new BaseType("number"), new BaseType("set")); break;
                case 9: numberStack = (Stack<double>)v; type = new BaseType(new BaseType("number"), new BaseType("stack")); break;
                case 10: numberQueue = (Queue<double>)v; type = new BaseType(new BaseType("number"), new BaseType("queue")); break;
                case 11: text = (string)v; type = new BaseType("text"); break;
                case 12: textList = (List<string>)v; type = new BaseType(new BaseType("text"), new BaseType("list")); break;
                case 13: textSet = (HashSet<string>)v; type = new BaseType(new BaseType("text"), new BaseType("set")); break;
                case 14: textStack = (Stack<string>)v; type = new BaseType(new BaseType("text"), new BaseType("stack")); break;
                case 15: textQueue = (Queue<string>)v; type = new BaseType(new BaseType("text"), new BaseType("queue")); break;
                case 16: vertex = (Vertex)v; type = new BaseType("vertex"); break;
                case 17: vertexList = (List<Vertex>)v; type = new BaseType(new BaseType("vertex"), new BaseType("list")); break;
                case 18: vertexSet = (HashSet<Vertex>)v; type = new BaseType(new BaseType("vertex"), new BaseType("set")); break;
                case 19: vertexStack = (Stack<Vertex>)v; type = new BaseType(new BaseType("vertex"), new BaseType("stack")); break;
                case 20: vertexQueue = (Queue<Vertex>)v; type = new BaseType(new BaseType("vertex"), new BaseType("queue")); break;
                case 21: edge = (Edge)v; type = new BaseType("edge"); break;
                case 22: edgeList = (List<Edge>)v; type = new BaseType(new BaseType("edge"), new BaseType("list")); break;
                case 23: edgeSet = (HashSet<Edge>)v; type = new BaseType(new BaseType("edge"), new BaseType("set")); break;
                case 24: edgeStack = (Stack<Edge>)v; type = new BaseType(new BaseType("edge"), new BaseType("stack")); break;
                case 25: edgeQueue = (Queue<Edge>)v; type = new BaseType(new BaseType("edge"), new BaseType("queue")); break;
                case 26: identifyObject(v); break;
                case 27: identifyObject(v); break;
                case 28: identifyObject(v); break;
                case 29: identifyObject(v); break;
                default: throw new Exception(v.GetType() + " is not supported!");
            }
        }

        private void identifyObject(object v)
        {
            if (v.GetType() == typeof(List<object>))
            {
                List<object> l = (List<object>)v;
                if (l[0].GetType() == typeof(bool))
                {
                    List<bool> b = new List<bool>();
                    l.ForEach(bo => {
                        b.Add((bool)bo);
                    });
                    setObjectAndType(b);
                }
                else
                if (l[0].GetType() == typeof(double))
                {
                    List<double> b = new List<double>();
                    l.ForEach(bo => {
                        b.Add((double)bo);
                    });
                    setObjectAndType(b);
                }
                else
                if (l[0].GetType() == typeof(string))
                {
                    List<string> b = new List<string>();
                    l.ForEach(bo => {
                        b.Add((string)bo);
                    });
                    setObjectAndType(b);
                }
                else
                if (l[0].GetType() == typeof(Vertex))
                {
                    List<Vertex> b = new List<Vertex>();
                    l.ForEach(bo => {
                        b.Add((Vertex)bo);
                    });
                    setObjectAndType(b);
                }
                else
                if (l[0].GetType() == typeof(Edge))
                {
                    List<Edge> b = new List<Edge>();
                    l.ForEach(bo => {
                        b.Add((Edge)bo);
                    });
                    setObjectAndType(b);
                }
            }
            else
            if (v.GetType() == typeof(HashSet<object>))
            {
                HashSet<object> l = (HashSet<object>)v;
                if (l.GetEnumerator().Current.GetType() == typeof(bool))
                {
                    HashSet<bool> b = new HashSet<bool>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Add((bool)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.GetEnumerator().Current.GetType() == typeof(double))
                {
                    HashSet<double> b = new HashSet<double>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Add((double)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.GetEnumerator().Current.GetType() == typeof(string))
                {
                    HashSet<string> b = new HashSet<string>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Add((string)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.GetEnumerator().Current.GetType() == typeof(Vertex))
                {
                    HashSet<Vertex> b = new HashSet<Vertex>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Add((Vertex)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.GetEnumerator().Current.GetType() == typeof(Edge))
                {
                    HashSet<Edge> b = new HashSet<Edge>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Add((Edge)e.Current);
                    setObjectAndType(b);
                }
            }
            else
            if (v.GetType() == typeof(Stack<object>))
            {
                Stack<object> l = (Stack<object>)v;
                if (l.Peek().GetType() == typeof(bool))
                {
                    Stack<bool> b = new Stack<bool>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Push((bool)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(double))
                {
                    Stack<double> b = new Stack<double>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Push((double)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(string))
                {
                    Stack<string> b = new Stack<string>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Push((string)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(Vertex))
                {
                    Stack<Vertex> b = new Stack<Vertex>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Push((Vertex)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(Edge))
                {
                    Stack<Edge> b = new Stack<Edge>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Push((Edge)e.Current);
                    setObjectAndType(b);
                }
            }
            else
            if (v.GetType() == typeof(Queue<object>))
            {
                Queue<object> l = (Queue<object>)v;
                if (l.Peek().GetType() == typeof(bool))
                {
                    Queue<bool> b = new Queue<bool>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Enqueue((bool)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(double))
                {
                    Queue<double> b = new Queue<double>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Enqueue((double)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(string))
                {
                    Queue<string> b = new Queue<string>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Enqueue((string)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(Vertex))
                {
                    Queue<Vertex> b = new Queue<Vertex>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Enqueue((Vertex)e.Current);
                    setObjectAndType(b);
                }
                else
                if (l.Peek().GetType() == typeof(Edge))
                {
                    Queue<Edge> b = new Queue<Edge>();
                    IEnumerator e = l.GetEnumerator();
                    while (e.MoveNext())
                        b.Enqueue((Edge)e.Current);
                    setObjectAndType(b);
                }
            }
        }
    }
}
