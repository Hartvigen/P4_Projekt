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
        public static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            const string defaultFile = "MAGIAFile.txt";

            //Uncomment these lines if you wanna play with the program            
            Console.WriteLine("Doing custom work!");
            var customArgs = new string[2];
            args = customArgs;
            args[0] = "-c";
            args[1] = defaultFile;

            if (args.Length > 0)
            {
                if (!TryParse(args[1], out var parser))
                {
                    Console.WriteLine("Couldn't even parse the file!");
                }else
                    switch (args[0])
                {
                    case "-c":
                    case "--compile":
                        Console.WriteLine("Doing a complete compile on " + args[1]);
                        parser.tab.name = "top";
                        var list = new List<Visitor> {new Cleaner(parser.tab), new AttributeMover(parser.tab), new ScopeChecker(parser.tab), new TypeChecker(parser.tab)};
                        ApplyVisitors(list, args[1]);
                        var executor = new Executor(parser);
                        Console.WriteLine(executor.ErrorList.Count == 0 ? "Done" : "Execution Failed");
                        break;
                    case "-h":
                    case "--help":
                        Console.WriteLine("Compile file: MagiaC.exe [filePath]");
                        Console.WriteLine("For help: MagiaC.exe -h || MagiaC.exe --help");
                        Console.WriteLine("PrettyPrint AST: MagiaC.exe -p [filepath] || MagiaC.exe --prettyprint [filepath]");
                        Console.WriteLine("XmlTree AST: MagiaC.exe -x [filepath] || --xmlprint [filepath]");
                        Console.WriteLine("Create Test Png: MagiaC.exe -t || MagiaC.exe --test");
                        Console.WriteLine("If no arguments are given the compiler will look for default file called: \"" + defaultFile + "\" in its directory and compile compile that.");
                        break;
                    case "-p":
                    case "--prettyprint":
                        parser.tab.name = "top";
                        var list1 = new List<Visitor> { new Cleaner(parser.tab), new AttributeMover(parser.tab), new ScopeChecker(parser.tab), new TypeChecker(parser.tab), new PrettyPrinter() };
                        ApplyVisitors(list1, args[1]);
                        Console.WriteLine("Done");
                        break;
                    case "-x":
                    case "--xmlprint":
                        Console.WriteLine("Parsing input file and printing XML: " + args[1]);
                        Console.WriteLine(ApplyVisitor(new XmlTreeBuilder(),args[1]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                    case "-t":
                    case "--test":
                        Console.WriteLine("Printing test png called: test.png ");
                        Console.WriteLine(DotToPng.CreatePNGFile() ? "print succeeded!" : "print failed!");
                        break;
                    default:
                        Console.WriteLine("Parsing input file: " + args[0]);
                        Console.WriteLine(TryParse(args[0]) ? "Compile succeeded!" : "Compile failed!");
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

        private static bool TryParse(string filePath, out Parser parser)
        {
            parser = new Parser(new Scanner(filePath));
            parser.Parse();
            return parser.errors.count == 0;
        }
        
        private static bool TryParse(string filePath)
        {
            var parser = new Parser(new Scanner(filePath));
            parser.Parse();
            return parser.errors.count == 0;
        }

        private static bool ApplyVisitor(Visitor visitor, string inputFilePath)
        {
            if (TryParse(inputFilePath, out var parser))
            {
                parser.mainNode.Accept(visitor);
                if (visitor.ErrorList.Count == 0)
                    File.WriteAllText(visitor.AppropriateFileName, visitor.Result.ToString());
                else
                {
                    Console.WriteLine("-----------ERRORS-----------");
                    visitor.ErrorList.ForEach(Console.WriteLine);
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void ApplyVisitors(IEnumerable<Visitor> visitors, string inputFilePath)
        {
            if (!TryParse(inputFilePath, out var parser)) return;
            foreach(var vi in visitors)
            {
                parser.mainNode.Accept(vi);
                if (vi.ErrorList.Count == 0)
                    File.WriteAllText(vi.AppropriateFileName, vi.Result.ToString());
                else
                {
                    PrintErrors(vi);
                    break; //We break out of the loop as the other visitors cannot be relied upon if errors was found the visitor.
                }
            }
        }

        private static void PrintErrors(Visitor vi) {
            //If there are no errors we print nothing.
            if (vi.ErrorList.Count == 0)
                return;

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
            else Console.WriteLine(separator + vi.GetType().Name + separator);
            vi.ErrorList.ForEach(Console.WriteLine);
        }
    }
}
