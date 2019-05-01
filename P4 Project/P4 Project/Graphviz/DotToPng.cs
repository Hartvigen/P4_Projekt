using System.IO;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace P4_Project.Graphviz
{
    public abstract class DotToPng
    {
        public const string DefaultFilePath = "defualt.png";
        public const string DefaultDotCode = "digraph{one -> two; two -> three; three -> four; four -> one;}";

        public static bool CreatePngFile(string dot = DefaultDotCode, string fileName = DefaultFilePath)
        {
            var output = Setup().GenerateGraph(dot, Enums.GraphReturnType.Png);
            File.WriteAllBytes(fileName, output);
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
    }
}
