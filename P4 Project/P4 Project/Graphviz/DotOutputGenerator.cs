using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Graphviz
{
    public abstract class DotOutputGenerator
    {
        public static string printMode = null;
        
        public static readonly string DefaultFilePath = AppDomain.CurrentDomain.BaseDirectory + "default.png";
        public const string DefaultDotCode = "digraph{one -> two; two -> three; three -> four; four -> one;}";
        private static bool _directoryCreated;
        private static string _currentDirectory;
        private static int _pictureNumber;
        private static readonly List<string> FinishedDotHistory = new List<string>();

        private static void CreateCustomDotFile(string dot, string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(dot, fileName);
        }

        public static void CreateCustomPngFile(string dot, string fileName)
        {
            var output = Setup().GenerateGraph(dot, Enums.GraphReturnType.Png);
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllBytes(fileName, output);
        }

        public static bool CreateDefaultPngFile()
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

        /// <summary>
        /// Creates output from a list of vertex and depending on the printMode it can be either made into
        /// Dot code or into a PNG file.
        /// </summary>
        /// <param name="executorScene">The list of vertex to be printed. the vertex contains edges if any.</param>
        /// <exception cref="Exception">Throws an exception if printMode is invalid or if you call the program twice within the same millisecond</exception>
        public static void CreateOutputFromScene(List<Vertex> executorScene)
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
                        s.Append(keyValuePair.Key + " = " + "\"" + keyValuePair.Value.o + "\"" + ", ");
                    }
                }
                //To remove the last comma and space.
                s.AppendLine("]");
            });

            //Sets up the place for all undirected edges to be placed!
            s.AppendLine("subgraph undirected {");
            s.AppendLine("edge [dir=none]");

            //Every edge is found and if it is undirected we append it here.
            HashSet<Edge> processed = new HashSet<Edge>();
            executorScene.ForEach(v =>
            {
                v.edges.ForEach(e =>
                {
                    if (e.opera != Operators.Nonarr || processed.Contains(e)) return;
                    processed.Add(e);

                    s.Append(e.from.name);
                    s.Append(" -> ");
                    s.Append(e.to.name + " [");
                    //For every mapped attribute we pass the value to DOT.
                    foreach (var keyValuePair in e.attributes)
                    {
                        if (PreDefined.PreDefinedAttributesEdge.Contains(keyValuePair.Key))
                        {
                            s.Append(keyValuePair.Key + " = " + "\"" + (string) keyValuePair.Value.o + "\"" + ", ");
                        }
                    }
                    //To remove the last comma and space.
                    s.AppendLine("]");
                });
            });
            //Close the undirected graph
            s.AppendLine("}");
            
            //Open directed graph
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
                            s.Append(keyValuePair.Key + " = " + "\"" + (string) keyValuePair.Value.o + "\"" + ", ");
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
            FinishedDotHistory.Add(finishedDot);

            if (!_directoryCreated)
            {
                _currentDirectory = "" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + $"{DateTime.Now.Millisecond}";
                if (Directory.Exists(_currentDirectory))
                    throw new Exception("You called the program twice within the same millisecond please dont.");
                Directory.CreateDirectory(_currentDirectory);
                _directoryCreated = true;
            }
            
            var fileName = AppDomain.CurrentDomain.BaseDirectory + "/" + _currentDirectory + "/" + "graph-" + _pictureNumber++;
            switch (printMode)
            {   
                case "dot":
                    fileName += ".gv";
                    CreateCustomDotFile(finishedDot, fileName);
                    break;
                case "png":
                    fileName += ".png";
                    CreateCustomPngFile(finishedDot, fileName);
                    break;
                default:
                    throw new Exception($"The print mode '{printMode ?? "null"}' is not valid");
            }
        }
        /// <summary>
        /// Used in testing to get a history of all the printed Dot code
        /// </summary>
        /// <returns>Returns a list of strings where each string is dot code produced from a print statement.</returns>
        public static List<string> GetOutputs()
        {
            return FinishedDotHistory;
        }
    }
}
