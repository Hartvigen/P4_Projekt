using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Graphviz
{
    public abstract class DotToPng
    {
        public static readonly string defaultFilePath = AppDomain.CurrentDomain.BaseDirectory + "default.png";
        public const string defaultDotCode = "digraph{one -> two; two -> three; three -> four; four -> one;}";

        public static bool CreatePNGFile(string DOT, string fileName)
        {
            var output = setup().GenerateGraph(DOT, Enums.GraphReturnType.Png);
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllBytes(fileName, output);
            return true;
        }

        public static bool CreatePNGFile()
        {
            var output = setup().GenerateGraph(defaultDotCode, Enums.GraphReturnType.Png);
            if (File.Exists(defaultFilePath))
                File.Delete(defaultFilePath);
            File.WriteAllBytes(defaultFilePath, output);
            return true;
        }

        private static GraphGeneration setup()
        {
            return new GraphGeneration(
                new GetStartProcessQuery(),
                new GetProcessStartInfoQuery(),
                new RegisterLayoutPluginCommand(
                    new GetProcessStartInfoQuery(),
                    new GetStartProcessQuery())
                );
        }

        public static void CreatePNGFileFromScene(List<Vertex> executorScene)
        {
            var s = new StringBuilder();
            //Every graph is a directed graph and if a undirected edge
            //is encountered it is style to not look like it on a sub graph.
            s.AppendLine("digraph {");
            
            //All vertices are placed globally accessible by their identifier. 
            executorScene.ForEach(v =>
            {
                s.Append(v.identifyer);
                s.Append(" [");
                foreach (var keyValuePair in v.attributes)
                {
                    if (PreDefined.preDefinedAttributesVertex.Contains(keyValuePair.Key))
                    {
                        s.Append(keyValuePair.Key + " = " + (string) keyValuePair.Value.o + ", ");
                    }
                }
                //To remove the last comma and space.
                s.Length -= 2;
                s.AppendLine("]");
            });

            //Sets up the place for all undirected edges to be placed!
            s.AppendLine("subgraph undirected {");
            s.AppendLine("edge [dir=none]");
            
            //Every edge is found and if it is undirected we append it here.
            executorScene.ForEach(v =>
            {
                v.edge.ForEach(e =>
                {
                    if (e.opera != Operators.Nonarr) return;
                    s.Append(e.from.identifyer);
                    s.Append(" -> ");
                    s.Append(e.to.identifyer + " [");
                    //For every mapped attribute we pass the value to DOT.
                    foreach (var keyValuePair in e.attributes)
                    {
                        if (PreDefined.preDefinedAttributesEdge.Contains(keyValuePair.Key))
                        {
                            s.Append(keyValuePair.Key + " = " + (string) keyValuePair.Value.o + ", ");
                        }
                    }
                    //To remove the last comma and space.
                    s.Length -= 2;
                    s.AppendLine("]");
                });
            });
            //Close the undirected graph
            s.AppendLine("}");
            
            s.AppendLine("subgraph directed {");

            executorScene.ForEach(v =>
            {
                v.edge.ForEach(e =>
                {
                    switch (e.opera)
                    {
                        case Operators.Rightarr:
                            s.Append(e.@from.identifyer);
                            s.Append(" -> ");
                            s.Append(e.to.identifyer + " [");
                            break;
                        case Operators.Leftarr:
                            s.Append(e.to.identifyer);
                            s.Append(" -> ");
                            s.Append(e.@from.identifyer + " [");
                            break;
                        default: return;
                    }
                    
                    //For every mapped attribute we pass the value to DOT.
                    foreach (var keyValuePair in e.attributes)
                    {
                        if (PreDefined.preDefinedAttributesEdge.Contains(keyValuePair.Key))
                        {
                            s.Append(keyValuePair.Key + " = " + (string) keyValuePair.Value.o + ", ");
                        }
                    }
                    //To remove the last comma and space.
                    s.Length -= 2;
                    s.AppendLine("]");
                });
            });
            
            //Close the directed graph
            s.AppendLine("}");

            //The main graph is ended.
            s.AppendLine("}");

            CreatePNGFile(s.ToString(), AppDomain.CurrentDomain.BaseDirectory + "done.png");
        }
    }
}
