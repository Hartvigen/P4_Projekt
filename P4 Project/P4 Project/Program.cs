using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.IO;
using P4_Project.Graphviz;
using System.Collections.Generic;
using System.Text;
using P4_Project.Compiler.Interpreter;
using P4_Project.Compiler.SemanticAnalysis.Visitors;
using P4_Project.Compiler.SemanticAnalysis.Visitors.Extra;

namespace P4_Project
{
    public static class Program
    {
        private static Parser _parser;


        public static void Main(string[] args)
        {            
            switch (args.Length > 0 ? args[0] : "-h")
            {
                case "-i":
                case "--image":
                    TryParse(args[1]);
                    DotOutputGenerator.printMode = "png";
                    Console.WriteLine("Doing a complete compile on " + args[1]);
                    _parser.tab.name = "top";
                    Console.WriteLine(ApplyVisitors(
                        new List<Visitor>
                        {                                    
                                    new Cleaner(_parser.tab),
                                    new AttributeMover(_parser.tab),
                                    new ScopeChecker(_parser.tab),
                                    new TypeChecker(_parser.tab),
                                    new Interpreter(_parser.tab)
                        }, args[1]) ? "Compile succeeded!" : "Compile failed!");
                    break;

                case "-d":
                case "--dot":
                    TryParse(args[1]);
                    DotOutputGenerator.printMode = "dot";
                    Console.WriteLine("Doing a complete compile on " + args[1] + " and delivering dot code.");
                    _parser.tab.name = "top";
                    Console.WriteLine(ApplyVisitors(
                        new List<Visitor>
                        {
                                    new Cleaner(_parser.tab),
                                    new AttributeMover(_parser.tab),
                                    new ScopeChecker(_parser.tab),
                                    new TypeChecker(_parser.tab),
                                    new Interpreter(_parser.tab)
                        }, args[1]) ? "Compile succeeded!" : "Compile failed!");
                    break;

                case "-p":
                case "--prettyprint":
                    TryParse(args[1]);
                    _parser.tab.name = "top";
                    Console.WriteLine(ApplyVisitors(
                        new List<Visitor>
                        {
                                    new Cleaner(_parser.tab),
                                    new AttributeMover(_parser.tab),
                                    new ScopeChecker(_parser.tab),
                                    new TypeChecker(_parser.tab),
                                    new PrettyPrinter()
                        }, args[1]) ? "Compile succeeded!" : "Compile failed!");
                    break;

                case "-x":
                case "--xmlprint":
                    TryParse(args[1]);
                    Console.WriteLine("Parsing input file and printing XML: " + args[1]);
                    Console.WriteLine(ApplyVisitors(
                        new List<Visitor>
                        {
                                    new XmlTreeBuilder()
                        }, args[1]) ? "Compile succeeded!" : "Compile failed!");
                    break;

                case "-t":
                case "--test":
                    Console.WriteLine("Printing test png called: test.png ");
                    Console.WriteLine(DotOutputGenerator.CreateDefaultPngFile() ? "print succeeded!" : "print failed!");
                    break;
                case "-h":
                case "--help":
                default:
                    Console.WriteLine("Compile file into DOT: MagiaC.exe -d [filePath] || MagiaC.exe --dot [filePath]");
                    Console.WriteLine("Compile file into images: MagiaC.exe -i [filePath] || MagiaC.exe --image [filePath]");
                    Console.WriteLine("For help: MagiaC.exe -h || MagiaC.exe --help");
                    Console.WriteLine("PrettyPrint AST: MagiaC.exe -p [filepath] || MagiaC.exe --prettyprint [filepath]");
                    Console.WriteLine("XmlTree AST: MagiaC.exe -x [filepath] || --xmlprint [filepath]");
                    Console.WriteLine("Create Test Png: MagiaC.exe -t || MagiaC.exe --test");
   
                    break;
            }
            Console.ReadKey();
        }

        private static bool TryParse(string filePath)
        {
            _parser = new Parser(new Scanner(filePath));
            _parser.Parse();
            return _parser.errors.count == 0;
        }
        
        private static bool ApplyVisitors(IEnumerable<Visitor> visitors, string inputFilePath)
        {
            if (!TryParse(inputFilePath)) 
                return false;

            foreach(var vi in visitors)
            {
                _parser.mainNode.Accept(vi);
                if (vi.ErrorList.Count == 0)
                    File.WriteAllText(vi.AppropriateFileName, vi.Result.ToString());
                else
                {
                    PrintErrors(vi);
                    return false;
                }
            }
            return true;
        }

        private static void PrintErrors(Visitor vi) {
            //Check if one or more.
            var error = "ERRORS";
            if (vi.ErrorList.Count == 1)
                error = "ERROR";

            var separator = "--------------------";
            var fl = separator + error + separator;
            Console.WriteLine(fl);

            //Calculates correct Separator length dependent on name size
            var i = vi.GetType().Name.Length;
            var j = fl.Length;

            var str = new StringBuilder();
            for (var k = (j - i) / 2; k > 0; k--) {
                str.Append("-");
            }

            separator = str.ToString();

            if(str.Length * 2 + i != j)
                Console.WriteLine(separator + vi.GetType().Name + separator + "-");
            else 
                Console.WriteLine(separator + vi.GetType().Name + separator);

            vi.ErrorList.ForEach(Console.WriteLine);
        }
    }
}
