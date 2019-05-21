using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Graphviz
{
    public abstract class DotToPng
    {
        public static readonly string DefaultFilePath = AppDomain.CurrentDomain.BaseDirectory + "default.png";
        public const string DefaultDotCode = "digraph{one -> two; two -> three; three -> four; four -> one;}";
        private static bool _directoryCreated;
        private static string _currentDirectory;
        private static int _pictureNumber;

        public static void CreatePngFile(string dot, string fileName)
        {
            var output = Setup().GenerateGraph(dot, Enums.GraphReturnType.Png);
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllBytes(fileName, output);
        }

        public static bool CreatePngFile()
        {
            var output = Setup().GenerateGraph(DefaultDotCode, Enums.GraphReturnType.Png);
            if (File.Exists(DefaultFilePath))
                File.Delete(DefaultFilePath);
            File.WriteAllBytes(DefaultFilePath, output);
            return true;
        }

        private static GraphGeneration Setup()
        {
            return new GraphGeneration(
                new GetStartProcessQuery(),
                new GetProcessStartInfoQuery(),
                new RegisterLayoutPluginCommand(
                    new GetProcessStartInfoQuery(),
                    new GetStartProcessQuery())
                );
        }

        public static void CreatePngFileFromScene(List<Vertex> executorScene)
        {
            var s = new StringBuilder();
            //Every graph is a directed graph and if a undirected edge
            //is encountered it is style to not look like it on a sub graph.
            s.AppendLine("digraph {");

            //All vertices are placed globally accessible by their identifier. (string)v.attributes["label"].o)
            executorScene.ForEach(v =>
            {
                v.name = PreDefined.NextUniqueString();
                s.Append(v.name);
                s.Append(" [");
                foreach (var keyValuePair in v.attributes)
                {
                    if (PreDefined.PreDefinedAttributesVertex.Contains(keyValuePair.Key))
                    {
                        s.Append(keyValuePair.Key + " = " + keyValuePair.Value.o + ", ");
                    }
                }
                //To remove the last comma and space.
                s.AppendLine("]");
            });

            //Sets up the place for all undirected edges to be placed!
            s.AppendLine("subgraph undirected {");
            s.AppendLine("edge [dir=none]");
            
            //Every edge is found and if it is undirected we append it here.
            executorScene.ForEach(v =>
            {
                v.edges.ForEach(e =>
                {
                    if (e.opera != Operators.Nonarr) return;
                    s.Append(e.from.name);
                    s.Append(" -> ");
                    s.Append(e.to.name + " [");
                    //For every mapped attribute we pass the value to DOT.
                    foreach (var keyValuePair in e.attributes)
                    {
                        if (PreDefined.PreDefinedAttributesEdge.Contains(keyValuePair.Key))
                        {
                            s.Append(keyValuePair.Key + " = " + (string) keyValuePair.Value.o + ", ");
                        }
                    }
                    //To remove the last comma and space.
                    s.AppendLine("]");
                });
            });
            //Close the undirected graph
            s.AppendLine("}");
            
            s.AppendLine("subgraph directed {");

            executorScene.ForEach(v =>
            {
                v.edges.ForEach(e =>
                {
                    switch (e.opera)
                    {
                        case Operators.Rightarr:
                            s.Append(e.from.name);
                            s.Append(" -> ");
                            s.Append(e.to.name + " [");
                            break;
                        case Operators.Leftarr:
                            s.Append(e.to.name);
                            s.Append(" -> ");
                            s.Append(e.from.name + " [");
                            break;
                        default: return;
                    }
                    
                    //For every mapped attribute we pass the value to DOT.
                    foreach (var keyValuePair in e.attributes)
                    {
                        if (PreDefined.PreDefinedAttributesEdge.Contains(keyValuePair.Key))
                        {
                            s.Append(keyValuePair.Key + " = " + (string) keyValuePair.Value.o + ", ");
                        }
                    }
                    //To remove the last comma and space.
                    s.AppendLine("]");
                });
            });
            
            //Close the directed graph
            s.AppendLine("}");

            //The main graph is ended.
            s.AppendLine("}");

            var finishedDot = s.ToString();

            if (!_directoryCreated)
            {
                _currentDirectory = "" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + $"{DateTime.Now.Millisecond}";
                if (Directory.Exists(_currentDirectory))
                    throw new Exception("You called the program twice within the same millisecond please dont.");
                Directory.CreateDirectory(_currentDirectory);
                _directoryCreated = true;
            }

            CreatePngFile(finishedDot,  AppDomain.CurrentDomain.BaseDirectory + "/" + _currentDirectory + "/" + "graph-" + _pictureNumber++ + ".png");
        }
    }
}
