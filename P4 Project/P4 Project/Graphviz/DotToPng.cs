using System;
using System.IO;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace P4_Project.Graphviz
{
    public abstract class DotToPng
    {
        public static bool createPNGFile()
        {
            String dotExample = "digraph{one -> two; two -> three; three -> four; four -> one;}";
            byte[] output = setup().GenerateGraph(dotExample, Enums.GraphReturnType.Png);
            File.WriteAllBytes("testgraph.png", output);
            return true;
        }

        public static bool createPNGFile(string DOT)
        {
            byte[] output = setup().GenerateGraph(DOT, Enums.GraphReturnType.Png);
            File.WriteAllBytes("graph.png", output);
            return true;
        }

        public static bool createPNGFile(string DOT, string fileName)
        {
            byte[] output = setup().GenerateGraph(DOT, Enums.GraphReturnType.Png);
            File.WriteAllBytes(fileName, output);
            return true;
        }

        private static GraphGeneration setup()
        {
            return new GraphGeneration(
                new GetStartProcessQuery(),
                new GetProcessStartInfoQuery(),
                new RegisterLayoutPluginCommand(new GetProcessStartInfoQuery(), new GetStartProcessQuery())
                );
        }
    }
}
