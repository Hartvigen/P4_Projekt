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
        const string defaultFile = "MAGIAFile.txt";
        private static Parser _parser;


        public static void Main(string[] args)
        {
            //Uncomment these lines if you wanna play with the program            
            if (args.Length == 0)
                args = new string[] { "-i", "MAGIAFile.txt" };

            if (args.Length > 0)
            {
                if (!TryParse(args[1]))
                {
                    Console.WriteLine("Couldn't even parse the file!");
                }
                else
                    switch (args[0])
                    {
                        case "-i":
                        case "--image":
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
                                },args[1]) ? "Compile succeeded!" : "Compile failed!");                        
                            break;

                        case "-d":
                        case "--dot":
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
                                },args[1]) ? "Compile succeeded!" : "Compile failed!");                        
                            break;

                        case "-p":
                        case "--prettyprint":
                            _parser.tab.name = "top";
                            Console.WriteLine(ApplyVisitors(
                                new List<Visitor>
                                {
                                    new Cleaner(_parser.tab), 
                                    new AttributeMover(_parser.tab), 
                                    new ScopeChecker(_parser.tab), 
                                    new TypeChecker(_parser.tab), 
                                    new PrettyPrinter()
                                },args[1]) ? "Compile succeeded!" : "Compile failed!");                        
                            break;

                        case "-x":
                        case "--xmlprint":
                            Console.WriteLine("Parsing input file and printing XML: " + args[1]);
                            Console.WriteLine(ApplyVisitors(
                                new List<Visitor>
                                {
                                    new XmlTreeBuilder()
                                },args[1]) ? "Compile succeeded!" : "Compile failed!");
                            break;

                        case "-t":
                        case "--test":
                            Console.WriteLine("Printing test png called: test.png ");
                            Console.WriteLine(DotOutputGenerator.CreateDefaultPngFile() ? "print succeeded!" : "print failed!");
                            break;

                        default:
                            Console.WriteLine("Compile file: MagiaC.exe [filePath]");
                            Console.WriteLine("For help: MagiaC.exe -h || MagiaC.exe --help");
                            Console.WriteLine("PrettyPrint AST: MagiaC.exe -p [filepath] || MagiaC.exe --prettyprint [filepath]");
                            Console.WriteLine("XmlTree AST: MagiaC.exe -x [filepath] || --xmlprint [filepath]");
                            Console.WriteLine("Create Test Png: MagiaC.exe -t || MagiaC.exe --test");
                            Console.WriteLine("If no arguments are given the compiler will look for default file called: \"" + defaultFile + "\" in its directory and compile compile that.");
                            break;
                    }
            }
            else if (File.Exists(defaultFile))
            {
                Console.WriteLine("Compiling standard file: " + defaultFile);
                Console.WriteLine(TryParse(defaultFile) ? "Compile succeeded!" : "Compile failed!");
            }
            else
            {
                Console.WriteLine("for help type: MagiaC.exe --help or: MagiaC.exe -h");
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
