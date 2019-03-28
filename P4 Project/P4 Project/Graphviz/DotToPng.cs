using System;
using System.IO;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace P4_Project.Graphviz
{
    public class DotToPng
    {
        private static GetStartProcessQuery getStartProcessQuery = new GetStartProcessQuery();
        private static GetProcessStartInfoQuery getProcessStartInfoQuery = new GetProcessStartInfoQuery();
        private static RegisterLayoutPluginCommand registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);
        private static GraphGeneration wrapper = new GraphGeneration(getStartProcessQuery, getProcessStartInfoQuery, registerLayoutPluginCommand);
        private bool isDone = false;

        public DotToPng()
        {
            createPNGFile();
        }

        public DotToPng(string DOT)
        {
            createPNGFileFromDOT(DOT);
        }

        public DotToPng(string DOT, string fileName)
        {
            createNamedPNGFileFromDOT(DOT, fileName);
        }

        public void createPNGFile()
        {
            String dotExample = "digraph{a -> b; b -> c; c -> a;}";
            byte[] output = wrapper.GenerateGraph(dotExample, Enums.GraphReturnType.Png);
            File.WriteAllBytes("testgraph.png", output);
            isDone = true;
        }

        public void createPNGFileFromDOT(string DOT)
        {
            byte[] output = wrapper.GenerateGraph(DOT, Enums.GraphReturnType.Png);
            File.WriteAllBytes("graph.png", output);
            isDone = true;
        }

        public void createNamedPNGFileFromDOT(string DOT, string fileName)
        {
            byte[] output = wrapper.GenerateGraph(DOT, Enums.GraphReturnType.Png);
            File.WriteAllBytes(fileName, output);
            isDone = true;
        }

        public bool getIsDone()
        {
            return isDone;
        }
    }
}
