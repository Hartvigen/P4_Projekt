using System;
using System.Collections.Generic;
using System.IO;
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
                 "GetEdge",
                "RemoveEdge",
                 "GetEdges",
              "RemoveVertex",
                 "GetVertices",
                "ClearEdges",
               "ClearAll",
               "Print",
               "AsText",
            "Add",
            "Terminal"
        };

        public static readonly List<string> PreDefinedAttributesVertex = new List<string>
        {
            "label",
            "color"
        };

        public static readonly List<string> PreDefinedAttributesEdge = new List<string>
        {
            "label",
            "color",
            "style"
        };

        public static object GetPreDefinedValueOfPreDefinedAttributeVertex(string name)
        {
            switch (name)
            {
                case "label":
                    return "vertex";
                case "color":
                    return "black";
                default:
                    throw new Exception("Attribute: " + name + " has no predefined value for vertex!");
            }
        }

        public static object GetPreDefinedValueOfPreDefinedAttributeEdge(string name)
        {
            switch (name)
            {
                case "label":
                    return "default";
                case "color":
                    return "black";
                case "style":
                    return "none";
                default:
                    throw new Exception("Attribute: " + name + " has no predefined value for edge!");
            }
        }

        public static object GetDefaultValueOfAttributeType(BaseType type)
        {
            switch (type.name)
            {
                case "number":
                    return 0.0;
                case "text":
                    return "";
                case "boolean":
                    return false;
                case "vertex":
                case "edge":
                case "collec":
                    return new NoneConst();
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
            if (!PreDefinedAttributesVertex.Contains(name))
                throw new Exception(name + " is not a predefined attribute for vertex");
            switch (name) {
                case "label": return "text";
                case "color": return "text";
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
            if (PreDefinedFunctions.Contains(function))
                switch (function)
                {
                    case "Print":
                        Print(executor);
                        break;
                    case "AsText":
                        AsText(parameters, executor);
                        break;
                    case "GetEdge":
                        GetEdge(parameters, executor);
                        break;
                    case "RemoveEdge":
                        RemoveEdge(parameters, executor);
                        break;
                    case "GetEdges":
                        GetEdges(executor);
                        break;
                    case "RemoveVertex":
                        RemoveVertex(parameters, executor);
                        break;
                    case "GetVertices":
                        GetVertices(executor);
                        break;
                    case "ClearEdges":
                        ClearEdges(executor);
                        break;
                    case "ClearAll":
                        ClearAll(executor);
                        break;
                    case "Add":
                        Add(parameters, executor);
                        break;
                    case "Terminal":
                        Terminal(parameters, executor);
                        break;
                    default: throw new Exception("Missing implementation of the PreDefFunction: " + function);
                }
            else throw new Exception(function + " is not a pre Defined Function!");
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
        private static void Print(Interpreter executor)
        {
            DotToPng.CreatePngFileFromScene(executor.scene);
        }

        private static void ClearAll(Interpreter executor)
        {
            executor.scene.Clear();
        }

        private static void ClearEdges(Interpreter executor)
        {
            executor.scene.ForEach(v => v.edges.Clear());
        }

        private static void GetVertices(Interpreter executor)
        {
            executor.currentValue = null;
            var vertexList = new List<Vertex>();
            foreach (var v in executor.scene)
                vertexList.Add(v);
            executor.currentValue = new Value(vertexList);
        }

        private static void RemoveVertex(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            executor.scene.Remove((Vertex)parameters[0].o);
        }

        private static void GetEdges(Interpreter executor)
        {
            executor.currentValue = null;
            var edgeList = new List<Edge>();
            foreach (var v in executor.scene)
                edgeList.AddRange(v.edges);
            executor.currentValue = new Value(edgeList);
        }
        private static void GetEdge(List<Value> parameters, Interpreter executor)
        {
            var v1 = (Vertex)parameters[0].o;
            var v2 = (Vertex)parameters[1].o;
            foreach (var v in executor.scene)
            {
                foreach (var e in v.edges)
                {
                    if (e.HasVertex(v1, v2))
                        executor.currentValue = new Value(e);
                }
            }
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

        private static void Terminal(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            Console.WriteLine(parameters[0].o.ToString());
        }

        private static void Add(IReadOnlyList<Value> parameters, Interpreter executor)
        {
            switch (parameters[0].type.collectionType.name)
            {
                case "list":
                    (parameters[0].o as List<object>).Add(parameters[1].o);
                    foreach (object o in (parameters[0].o as List<object>))
                    {
                        Console.WriteLine(o.ToString());
                    }
                    break;
                case "set":
                    (parameters[0].o as HashSet<object>).Add(parameters[1].o);
                    foreach (object o in (parameters[0].o as HashSet<object>))
                    {
                        Console.WriteLine(o.ToString());
                    }

                    break;
                default:
                    throw new Exception("Add only works with collections of type list or set");
            }

        }

        public static BaseType FindReturnTypeOfPreDefFunctions(string name, List<BaseType> parameters)
        {
            switch (name)
            {
                case "GetEdge": return new BaseType("edge");
                case "RemoveEdge": return new BaseType("none");
                case "GetEdges": return new BaseType(new BaseType("set"), new BaseType("edge"));
                case "RemoveVertex": return new BaseType("none");
                case "GetVertices": return new BaseType(new BaseType("set"), new BaseType("vertex"));
                case "ClearEdges": return new BaseType("none");
                case "ClearAll": return new BaseType("none");
                case "Print": return new BaseType("none");
                case "Terminal": return new BaseType("none");
                case "AsText": return new BaseType("text");
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
                case "Add": return new BaseType("none");
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }
        
        

        public static List<List<BaseType>> FindListOfParameterLists(string name)
        {
            switch (name)
            {
                case "GetEdge": return new List<List<BaseType>>{new List<BaseType> {new BaseType("vertex"), new BaseType("vertex")}};
                case "RemoveEdge": return new List<List<BaseType>>{new List<BaseType> {new BaseType("edge")}};
                case "RemoveVertex": return new List<List<BaseType>>{new List<BaseType> { new BaseType("vertex") }};
                case "GetEdges": return new List<List<BaseType>>{new List<BaseType>()};
                case "GetVertices": return new List<List<BaseType>>{new List<BaseType>()};
                case "ClearEdges": return new List<List<BaseType>>{new List<BaseType>()};
                case "ClearAll": return new List<List<BaseType>>{new List<BaseType>()};
                case "Print": return new List<List<BaseType>>{new List<BaseType>()};
                case "Terminal": return new List<List<BaseType>> { new List<BaseType> { new BaseType("text") } };
                case "AsText": return new List<List<BaseType>>
                {
                    new List<BaseType> { new BaseType("number") },
                    new List<BaseType> { new BaseType("boolean") },
                    new List<BaseType> { new BaseType("text") }
                };
                case "Add": return new List<List<BaseType>>
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
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }
    }
}
