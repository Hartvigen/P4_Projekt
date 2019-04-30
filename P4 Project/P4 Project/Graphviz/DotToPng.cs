using System;
using System.IO;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace P4_Project.Graphviz
{
    public abstract class DotToPng
    {
        public const string defaultFilePath = "defualt.png";
        public const string defaultDotCode = "digraph{one -> two; two -> three; three -> four; four -> one;}";

        public static bool createPNGFile(string DOT = defaultDotCode, string fileName = defaultFilePath)
        {
            byte[] output = setup().GenerateGraph(DOT, Enums.GraphReturnType.Png);
            File.WriteAllBytes(fileName, output);
            return true;
        }

        public static GraphGeneration setup()
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
