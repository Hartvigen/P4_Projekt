using System;
using System.Collections.Generic;
using P4_Project.AST;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Compiler.Interpreter
{
    public class Value
    {
        private Dictionary<Type, int> typeMap = new Dictionary<Type, int>();

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
            }
        }
    }
}
