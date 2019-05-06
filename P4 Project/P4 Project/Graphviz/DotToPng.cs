using System;
using System.IO;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P4_Project.Graphviz
{
    public abstract class DotToPng
    {
        public static string defaultFilePath = AppDomain.CurrentDomain.BaseDirectory + "defualt.png";
        public const string defaultDotCode = "digraph{one -> two; two -> three; three -> four; four -> one;}";

        public static bool CreatePNGFile(string DOT, string fileName)
        {
            byte[] output = setup().GenerateGraph(DOT, Enums.GraphReturnType.Png);
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllBytes(fileName, output);
            return true;
        }

        public static bool CreatePNGFile()
        {
            byte[] output = setup().GenerateGraph(defaultDotCode, Enums.GraphReturnType.Png);
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
    }
}
