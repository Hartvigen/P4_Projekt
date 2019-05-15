using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Executor;
using P4_Project.Compiler.Interpreter;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project
{
    public abstract class PreDefined
    {
        public static List<string> preDefinedFunctions = new List<string>
        {
                 "GetEdge",
                "RemoveEdge",
                 "GetEdges",
              "RemoveVertex",
                 "GetVertices",
                "ClearEdges",
                "ClearVertices",
               "ClearAll"
        };

        public static List<string> preDefinedAttributesVertex = new List<string>
        {
            "label",
            "color"
        };

        public static List<string> preDefinedAttributesEdge = new List<string>
        {
            "label",
            "color",
            "style"
        };

        public static object GetDefualtValueOfAttributeType(BaseType type)
        {
            if (type.name == "number")
                return 0.0;
            if (type.name == "text")
                return "";
            if (type.name == "boolean")
                return false;
            if (type.name == "vertex" || type.name == "edge" || type.name == "collec")
                return new NoneConst();
            throw new Exception("Type has no predefined value! :: " + type.name + " ::");
        }

        public static string GetTypeOfPreDefinedAttributeVertex(string name)
        {
            if (preDefinedAttributesVertex.Contains(name))
                switch (name) {
                    case "label": return "text";
                    case "color": return "text";
                    default: throw new Exception(name + " is a predefined attribute for vertex but no type has been specified for it");
                }
            throw new Exception(name + " is not a predefined attribute for vertex");
        }

        public static string GetTypeOfPreDefinedAttributeEdge(string name)
        {
            if (preDefinedAttributesEdge.Contains(name))
                switch (name)
                {
                    case "label": return "text";
                    case "color": return "text";
                    case "style": return "text";
                    default: throw new Exception(name + " is a predefined attribute for edge but no type has been specified for it");
                }
            throw new Exception(name + " is not a predefined attribute for edge");
        }

        internal static void DoPreDefFunction(string function, Executor executor, List<Value> parameters)
        {
            if (preDefinedFunctions.Contains(function))
                switch (function)
                {
                    case "GetEdge":
                        GetEdge(parameters, executor);
                        break;
                    case "RemoveEdge":
                        RemoveEdge(parameters, executor);
                        break;
                    case "GetEdges":
                        GetEdges(parameters, executor);
                        break;
                    case "RemoveVertex":
                        RemoveVertex(parameters, executor);
                        break;
                    case "GetVertices":
                        GetVertices(parameters, executor);
                        break;
                    case "ClearEdges":
                        ClearEdges(parameters, executor);
                        break;
                    case "ClearAll":
                        ClearAll(parameters, executor);
                        break;
                    default: throw new Exception("Missing implimentation of the PreDefFunction: " + function);
                }
            else throw new Exception(function + " is not a predef function!");
        }

        private static void ClearAll(List<Value> parameters, Executor executor)
        {
            executor.scene.Clear();
        }

        private static void ClearEdges(List<Value> parameters, Executor executor)
        {
            executor.scene.ForEach(v => v.edge.Clear());
        }

        private static void GetVertices(List<Value> parameters, Executor executor)
        {
            executor.currentValue = null;
            List<Vertex> vertexList = new List<Vertex>();
            foreach (Vertex v in executor.scene)
                vertexList.Add(v);
                executor.currentValue = new Value(vertexList);
        }

        private static void RemoveVertex(List<Value> parameters, Executor executor)
        {
            executor.scene.Remove((Vertex)parameters[0].o);
        }

        private static void GetEdges(List<Value> parameters, Executor executor)
        {
            executor.currentValue = null;
            List<Edge> edgeList = new List<Edge>();
            foreach (Vertex v in executor.scene)
                edgeList.AddRange(v.edge);
            executor.currentValue = new Value(edgeList);
        }
        private static void GetEdge(List<Value> parameters, Executor executor)
        {
            Vertex v1 = (Vertex)parameters[0].o;
            Vertex v2 = (Vertex)parameters[0].o;
            foreach (Vertex v in executor.scene)
            {
                foreach (Edge e in v.edge)
                {
                    if (e.hasVertex(v1, v2))
                        executor.currentValue = new Value(e);
                }
            }
        }

        private static void RemoveEdge(List<Value> parameters, Executor executor)
        {
            Edge eRemove = (Edge)parameters[0].o;
            foreach (Vertex v in executor.scene)
            {
                foreach (Edge e in v.edge)
                {
                    if (eRemove == e)
                        v.edge.Remove(e);
                }
            }
        }






    }
}
