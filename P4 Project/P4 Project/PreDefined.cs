using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using P4_Project.AST;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Interpreter;
using P4_Project.Compiler.Interpreter.Types;
using P4_Project.Graphviz;

namespace P4_Project
{
    public abstract class PreDefined
    {
        private static long _count;

        public static readonly List<string> PreDefinedFunctions = new List<string>
        {
        // General functions
        "Print",
        "AsText",
        "Terminal",
        //Collection functions
        "Clear",
        "Size",
        "IsEmpty",
        "Contains",
        "Add",
        "Push",
        "Enqueue",
        "Remove",
        "Pop",
        "Dequeue",
        "Get",
        "Peek",
        "Union",
        //Graph
        "GetEdge",
        "GetVertices",
        "GetEdges",
        "GetAdjacent",
        "RemoveVertex",
        "RemoveEdge",
        "ClearEdges",
        "ClearAll"
        };

        public static readonly List<string> PreDefinedAttributesVertex = new List<string>
        {
            "label",
            "color",
            "style"
        };

        public static readonly List<string> PreDefinedAttributesEdge = new List<string>
        {
            "label",
            "color",
            "style"
        };

        public static Value GetPreDefinedValueOfPreDefinedAttributeVertex(string name)
        {
            switch (name)
            {
                case "label":
                    return new Value("");
                case "color":
                    return new Value("black");
                case "style":
                    return new Value("none");
                default:
                    throw new Exception("Attribute: " + name + " has no predefined value for vertex!");
            }
        }

        public static Value GetPreDefinedValueOfPreDefinedAttributeEdge(string name)
        {
            switch (name)
            {
                case "label":
                    return new Value("");
                case "color":
                    return new Value("black");
                case "style":
                    return new Value("none");
                default:
                    throw new Exception("Attribute: " + name + " has no predefined value for edge!");
            }
        }

        public static Value GetDefaultValueOfAttributeType(BaseType type)
        {
            switch (type.name)
            {
                case "number":
                    return new Value(0.0);
                case "text":
                    return new Value("");
                case "boolean":
                    return new Value(false);
                case "vertex":
                case "edge":
                case "collec":
                    return new Value(new NoneConst());
                default:
                    throw new Exception("Type: " + type.name + " has no predefined value!");
            }
        }

        public static string NextUniqueString()
        {
            return "vertex" + _count++;
        }

        public static string GetTypeOfPreDefinedAttributeVertex(string name)
        {
            switch (name) {
                case "label": return "text";
                case "color": return "text";
                case "style": return "text";
                default: throw new Exception(name + " is a predefined attribute for vertex but no type has been specified for it");
            }
        }

        public static string GetTypeOfPreDefinedAttributeEdge(string name)
        {
            if (!PreDefinedAttributesEdge.Contains(name))
                throw new Exception(name + " is not a predefined attribute for edge");
            switch (name)
            {
                case "label": return "text";
                case "color": return "text";
                case "style": return "text";
                default: throw new Exception(name + " is a predefined attribute for edge but no type has been specified for it");
            }
        }

        internal static void DoPreDefFunction(string function, Interpreter executor, List<Value> parameters)
        {
            if (!PreDefinedFunctions.Contains(function))
                throw new Exception(function + " is not a pre-defined Function!");
            switch (function)
            {
                //General
                case "Print":
                    Print(executor);
                    break;
                case "AsText":
                    AsText(parameters, executor);
                    break;
                case "Terminal":
                    Terminal(parameters, executor);
                   break;
                    //Collections
                    case "Clear":
                        //TODO
                        Clear(parameters, executor);
                        break;
                    case "Size":
                        Size(parameters, executor);
                        break;
                    case "IsEmpty":
                        //TODO
                        IsEmpty(parameters, executor);
                        break;
                    case "Contains":
                        //TODO
                        Contains(parameters, executor);
                        break;
                    case "Add":
                        Add(parameters, executor);
                        break;
                    case "Push":
                        //TODO
                        Push(parameters, executor);
                        break;
                    case "Enqueue":
                        //TODO
                        Enqueue(parameters, executor);
                        break;
                    case "Remove":
                        Remove(parameters, executor);
                        break;
                    case "Pop":
                        //TODO
                        Pop(parameters, executor);                        
                        break;
                    case "Dequeue":
                        //TODO
                        Dequeue(parameters, executor);
                        break;
                    case "Get":
                        //TODO
                        Get(parameters, executor);
                        break;
                    case "Peek":
                        //TODO
                        Peek(parameters, executor);
                        break;
                    case "Union":
                        //TODO
                        Union(parameters, executor);
                        break;
                    //Graph
                    case "GetEdge":
                        GetEdge(parameters, executor);
                        break;
                    case "GetVertices":
                        GetVertices(executor);
                        break;
                    case "GetEdges":
                        GetEdges(executor);
                        break;
                    case "GetAdjacent":
                        GetAdjacent(parameters, executor);
                        break;
                    case "RemoveVertex":
                        RemoveVertex(parameters, executor);
                        break;
                    case "RemoveEdge":
                        RemoveEdge(parameters, executor);
                        break;
                    case "ClearEdges":
                        ClearEdges(executor);
                        break;
                    case "ClearAll":
                        ClearAll(executor);
                        break;                    
                    default: throw new Exception("Missing implementation of the PreDefFunction: " + function);
                }
        }

        /*
         * Implementation of general pre-defined functions
         */

        private static void Print(Interpreter executor)
        {
            DotOutputGenerator.CreateOutputFromScene(executor.scene);
        }

        private static void AsText(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            switch (parameters[0].type.name)
            {
                case "number":
                    executor.currentValue = new Value("" + (double)parameters[0].o);
                    break;
                case "boolean": 
                    executor.currentValue = new Value(((bool)parameters[0].o)? "true" : "false");
                    break;
                case "text":
                    executor.currentValue = new Value("" + parameters[0].o);
                    break;
                default: throw new Exception(parameters[0].type.name + " is not supported in AsText!");
            }
        }

        private static void Terminal(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            Console.WriteLine(parameters[0].o.ToString());
        }

        /*
        * Implementation of collection pre-defined functions
        */

        private static void Clear(List<Value> parameters, Interpreter executor)
        {
            ((List<object>)parameters[0].o).Clear();
        }

        private static void Size(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            executor.currentValue = new Value((double)((List<object>)parameters[0].o).Count);
        }

        private static void IsEmpty(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            executor.currentValue = new Value( ((List<object>) parameters[0].o).Count == 0? true : false);
        }

        private static void Contains(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            executor.currentValue = new Value(((List<object>) parameters[0].o).Contains(parameters[1].o));
        }

        private static void Add(List<Value> parameters, Interpreter executor)
        {
            ((List<object>)parameters[0].o).Add(parameters[1].o);
        }

        private static void Push(List<Value> parameters, Interpreter executor)
        {
            ((List<object>)parameters[0].o).Insert(0, parameters[1].o);
        }

        private static void Enqueue(List<Value> parameters, Interpreter executor)
        {
            ((List<object>)parameters[0].o).Add(parameters[1].o);
        }

        private static void Remove(List<Value> parameters, Interpreter executor)
        {
            if (parameters[0].type.collectionType.name == "list")
                ((List<object>)parameters[0].o).RemoveAt((int)((double)parameters[1].o) - 1);
            else if (parameters[0].type.collectionType.name == "set")
                ((List<object>)parameters[0].o).Remove(parameters[1].o);            
        }

        private static void Pop(List<Value> parameters, Interpreter executor)
        {
            executor.currentValue = new Value(((List<object>)parameters[0].o)[0]);
            ((List<object>)parameters[0].o).RemoveAt(0);
        }
        private static void Dequeue(List<Value> parameters, Interpreter executor)
        {
            executor.currentValue = new Value(((List<object>)parameters[0].o)[0]);
            ((List<object>)parameters[0].o).RemoveAt(0);
        }

        private static void Get(List<Value> parameters, Interpreter executor)
        {
            executor.currentValue = new Value(((List<object>)parameters[0].o)[(int)((double)parameters[1].o) - 1]);
        }

        private static void Peek(List<Value> parameters, Interpreter executor)
        {
            executor.currentValue = new Value(((List<object>)parameters[0].o)[0]);
        }

        private static void Union(List<Value> parameters, Interpreter executor)
        {
            foreach (var v in (List<object>)parameters[1].o)
            {
                if (!((List<object>)parameters[0].o).Contains(v))
                    ((List<object>)parameters[0].o).Add(v);
            }
        }
        /*
        * Implementation of graph pre-defined functions
        */


        private static void GetEdge(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            Vertex v1 = (Vertex)parameters[0].o;
            Vertex v2 = (Vertex)parameters[1].o;

            foreach (var e in v1.edges)
            {
                if (e.HasVertex(v1, v2) || (e.opera == Operators.Nonarr && e.HasVertex(v2, v1)))
                {
                    executor.currentValue = new Value(e);
                    return;
                }
            }

            executor.currentValue = new Value(new NoneConst());
        }

        private static void GetVertices(Interpreter executor)
        {
            executor.currentValue = null;
            var vertexList = new List<object>();
            foreach (var v in executor.scene)
                vertexList.Add(v);
            executor.currentValue = new Value(vertexList, new BaseType(new BaseType("vertex"), new BaseType("list")));
        }

        private static void GetEdges(Interpreter executor)
        {
            executor.currentValue = null;
            var edgeList = new List<object>();
            foreach (var v in executor.scene)
                edgeList.AddRange(v.edges.Where(e => !edgeList.Contains(e)));
            executor.currentValue = new Value(edgeList, new BaseType(new BaseType("edge"), new BaseType("list")));
        }

        private static void GetAdjacent(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            Vertex from = (parameters[0].o as Vertex);
            
            List<object> adjacentList = new List<object>();
            foreach(Edge adj in from.edges)
            {
                if (!adjacentList.Contains(adj.to) && adj.to != from)
                    adjacentList.Add(adj.to);
                else if (!adjacentList.Contains(adj.from) && adj.from != from)
                    adjacentList.Add(adj.from);
                else if (adj.to == adj.from && adj.from == from)
                    adjacentList.Add(from);
            }

            executor.currentValue = new Value(adjacentList, new BaseType(new BaseType("vertex"), new BaseType("list")));
        }

        private static void RemoveVertex(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            executor.scene.Remove((Vertex)parameters[0].o);
        }

        private static void RemoveEdge(List<Value> parameters, Interpreter executor)
        {
            var eRemove = (Edge)parameters[0].o;
            foreach (var v in executor.scene)
            {
                foreach (var e in v.edges)
                {
                    if (eRemove != e) continue;
                    v.edges.Remove(e);
                    return;
                }
            }
        }

        private static void ClearEdges(Interpreter executor)
        {
            executor.scene.ForEach(v => v.edges.Clear());
        }

        private static void ClearAll(Interpreter executor)
        {
            executor.scene.Clear();
        }

        public static BaseType FindReturnTypeOfPreDefFunctions(string name, List<BaseType> parameters)
        {
            switch (name)
            {
                //General
                case "Print": return new BaseType("none");
                case "AsText": return new BaseType("text");
                case "Terminal": return new BaseType("none");
                //Collection
                case "Clear": return new BaseType("none");
                case "Size": return new BaseType("number");
                case "IsEmpty": return new BaseType("boolean");
                case "Contains": return new BaseType("boolean");
                case "Add": return new BaseType("none");
                case "Push": return new BaseType("none");
                case "Enqueue": return new BaseType("none");
                case "Remove": return new BaseType("none");
                case "Pop":
                    switch (parameters[0].singleType.name)
                    {
                        case "vertex":
                            return new BaseType("vertex");
                        case "edge":
                            return new BaseType("edge");
                        case "number":
                            return new BaseType("number");
                        case "text":
                            return new BaseType("text");
                        case "boolean":
                            return new BaseType("boolean");
                        default: throw new Exception(parameters[0].singleType.name + " is not a valid type to Pop");
                    }
                case "Dequeue":
                    switch (parameters[0].singleType.name)
                    {
                        case "number": return new BaseType("number");
                        case "boolean": return new BaseType("boolean");
                        case "text": return new BaseType("text");
                        case "vertex": return new BaseType("vertex");
                        case "edge": return new BaseType("edge");
                        default: throw new Exception("A queue with element type " + parameters[0].singleType.name + " does not have a valid return type for Dequeue");
                    }
                case "Get":
                    switch (parameters[0].singleType.name)
                    {
                        case "number": return new BaseType("number");
                        case "boolean": return new BaseType("boolean");
                        case "text": return new BaseType("text");
                        case "vertex": return new BaseType("vertex");
                        case "edge": return new BaseType("edge");
                        default: throw new Exception("A list with element type " + parameters[0].singleType.name + " does not have a valid return type for Get");
                    }
                case "Peek":
                    switch (parameters[0].singleType.name)
                    {
                        case "number": return new BaseType("number");
                        case "boolean": return new BaseType("boolean");
                        case "text": return new BaseType("text");
                        case "vertex": return new BaseType("vertex");
                        case "edge": return new BaseType("edge");
                        default: throw new Exception("A " + parameters[0].collectionType.name + " with element type " + parameters[0].singleType.name + " does not have a valid return type for Peek");
                    }
                case "Union":
                    switch (parameters[0].singleType.name)
                    {
                        case "number": return new BaseType(new BaseType("number"), new BaseType("set"));
                        case "boolean": return new BaseType(new BaseType("boolean"), new BaseType("set"));
                        case "text": return new BaseType(new BaseType("text"), new BaseType("set"));
                        case "vertex": return new BaseType(new BaseType("vertex"), new BaseType("set"));
                        case "edge": return new BaseType(new BaseType("edge"), new BaseType("set"));
                        default: throw new Exception("A set with element type " + parameters[0].singleType.name + " does not have a valid return type for Union");
                    }                   
                //Graph
                case "GetEdge": return new BaseType("edge");
                case "GetVertices": return new BaseType(new BaseType("vertex"), new BaseType("list"));
                case "GetEdges": return new BaseType(new BaseType("edge"), new BaseType("list"));
                case "GetAdjacent": return new BaseType(new BaseType("vertex"), new BaseType("list"));
                case "RemoveVertex": return new BaseType("none");
                case "RemoveEdge": return new BaseType("none");
                case "ClearEdges": return new BaseType("none");
                case "ClearAll": return new BaseType("none");                
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }
        
        

        public static List<List<BaseType>> FindListOfParameterLists(string name)
        {
            switch (name)
            {
                //General
                case "Print": return new List<List<BaseType>> { new List<BaseType>() };
                case "AsText":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> { new BaseType("number") },
                        new List<BaseType> { new BaseType("boolean") },
                        new List<BaseType> { new BaseType("text") }
                    };
                case "Terminal": return new List<List<BaseType>> { new List<BaseType> { new BaseType("text") } };

                //Collection
                case "Clear":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue")) }
                    };
                case "Size":
                    return new List<List<BaseType>>
                {
                    new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("list")) },
                    new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("list")) },
                    new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("list")) },
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")) },
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")) },
                    new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("set")) },
                    new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("set")) },
                    new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("set")) },
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")) },
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")) },
                    new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("stack")) },
                    new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("stack")) },
                    new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("stack")) },
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack")) },
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack")) },
                    new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("queue")) },
                    new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("queue")) },
                    new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("queue")) },
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue")) },
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue")) }
                };
                case "IsEmpty":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")) },
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")) },
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack")) },
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue")) },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue")) }
                    };
                case "Contains":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("list")), new BaseType("number") },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("list")), new BaseType("boolean") },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("list")), new BaseType("text") },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")), new BaseType("vertex") },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")), new BaseType("edge") },

                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("set")), new BaseType("number") },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("set")), new BaseType("boolean") },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("set")), new BaseType("text") },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")), new BaseType("vertex") },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")), new BaseType("edge") },

                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("stack")), new BaseType("number") },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("stack")), new BaseType("boolean") },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("stack")), new BaseType("text") },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack")), new BaseType("vertex") },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack")), new BaseType("edge") },

                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("queue")), new BaseType("number") },
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("queue")), new BaseType("boolean") },
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("queue")), new BaseType("text") },
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue")), new BaseType("vertex") },
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue")), new BaseType("edge") }

                    };
                case "Add":
                    return new List<List<BaseType>>
                {
                    new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("list")), new BaseType("number")},
                    new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("list")), new BaseType("boolean")},
                    new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("list")), new BaseType("text")},
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")), new BaseType("vertex")},
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")), new BaseType("edge")},
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")), new BaseType("none")},
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")), new BaseType("none")},
                    new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("set")), new BaseType("number")},
                    new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("set")), new BaseType("boolean")},
                    new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("set")), new BaseType("text")},
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")), new BaseType("vertex")},
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")), new BaseType("edge")},
                    new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")), new BaseType("none")},
                    new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")), new BaseType("none")}
                };
                case "Push":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("stack")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("stack")), new BaseType("boolean")},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("stack")), new BaseType("text")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack")), new BaseType("vertex")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack")), new BaseType("edge")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack")), new BaseType("none")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack")), new BaseType("none")},
                    };
                case "Enqueue":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("queue")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("queue")), new BaseType("boolean")},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("queue")), new BaseType("text")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue")), new BaseType("vertex")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue")), new BaseType("edge")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue")), new BaseType("none")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue")), new BaseType("none")},
                    };
                case "Remove":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("set")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("set")), new BaseType("boolean")},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("set")), new BaseType("text")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")), new BaseType("vertex")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")), new BaseType("edge")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")), new BaseType("none")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")), new BaseType("none")}
                    };

                case "Pop":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack"))},
                    };
                case "Dequeue":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue"))},
                    };
                case "Get":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("list")), new BaseType("number")},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("list")), new BaseType("number")}
                    };
                case "Peek":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("stack"))},
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("queue"))},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("queue"))},
                    };
                case "union":
                    return new List<List<BaseType>>
                    {
                        new List<BaseType> {new BaseType(new BaseType("number"), new BaseType("set")), new BaseType(new BaseType("number"), new BaseType("set"))},
                        new List<BaseType> {new BaseType(new BaseType("boolean"), new BaseType("set")), new BaseType(new BaseType("boolean"), new BaseType("set"))},
                        new List<BaseType> {new BaseType(new BaseType("text"), new BaseType("set")), new BaseType(new BaseType("text"), new BaseType("set"))},
                        new List<BaseType> {new BaseType(new BaseType("vertex"), new BaseType("set")), new BaseType(new BaseType("vertex"), new BaseType("set"))},
                        new List<BaseType> {new BaseType(new BaseType("edge"), new BaseType("set")), new BaseType(new BaseType("edge"), new BaseType("set"))},


                    };
                    
                
                //Graph
                case "GetEdge": return new List<List<BaseType>>{new List<BaseType> {new BaseType("vertex"), new BaseType("vertex")}};
                case "GetVertices": return new List<List<BaseType>> { new List<BaseType>() };
                case "GetEdges": return new List<List<BaseType>> { new List<BaseType>() };
                case "GetAdjacent": return new List<List<BaseType>> { new List<BaseType> { new BaseType("vertex")} };
                case "RemoveVertex": return new List<List<BaseType>>{new List<BaseType> { new BaseType("vertex") }};
                case "RemoveEdge": return new List<List<BaseType>> { new List<BaseType> { new BaseType("edge") } };
                case "ClearEdges": return new List<List<BaseType>>{new List<BaseType>()};
                case "ClearAll": return new List<List<BaseType>>{new List<BaseType>()};
                
                
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }

        public static Dictionary<Type, int> typeMap = new Dictionary<Type, int>()
        {
            {typeof(NoneConst), 0},
            {typeof(bool), 1},
            {typeof(List<bool>), 2},
            {typeof(HashSet<bool>), 3},
            {typeof(Stack<bool>), 4},
            {typeof(Queue<bool>), 5},
            {typeof(double), 6},
            {typeof(List<double>), 7},
            {typeof(HashSet<double>), 8},
            {typeof(Stack<double>), 9},
            {typeof(Queue<double>), 10},
            {typeof(string), 11},
            {typeof(List<string>), 12},
            {typeof(HashSet<string>), 13},
            {typeof(Stack<string>), 14},
            {typeof(Queue<string>), 15},
            {typeof(Vertex), 16},
            {typeof(List<Vertex>), 17},
            {typeof(HashSet<Vertex>), 18},
            {typeof(Stack<Vertex>), 19},
            {typeof(Queue<Vertex>), 20},
            {typeof(Edge), 21},
            {typeof(List<Edge>), 22},
            {typeof(HashSet<Edge>), 23},
            {typeof(Stack<Edge>), 24},
            {typeof(Queue<Edge>), 25},
        };
    }
}
