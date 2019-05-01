using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Visitors;
using System;
using System.IO;
using P4_Project.Graphviz;

namespace P4_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            const string defaultFile = "MAGIAFile.txt";

            //Uncomment these lines if you wanna play with the program            
            Console.WriteLine("Doing custom work!");
            var customArgs = new string[2];
            args = customArgs;
            args[0] = "-s";
            args[1] = defaultFile;

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-s":
                    case "--symbolprint":
                        Console.WriteLine("Parsing input file and assigning variables: " + args[1]);
                        TryParse(args[1], out var parser);
                        var visitor = new TypeVisitor(parser);
                        TypeVisitor.Print();
                        Console.WriteLine(ApplyVisitor(visitor,args[1]) ? "Compile Succeeded!" : "Compile failed!");
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
                        Console.WriteLine("Parsing input file and PrettyPrinting: " + args[1]);
                        Console.WriteLine(ApplyVisitor(new PrettyPrinter(),args[1]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                    case "-x":
                    case "--xmlprint":
                        Console.WriteLine("Parsing input file and printing XML: " + args[1]);
                        Console.WriteLine(ApplyVisitor(new XmlTreeBuilder(),args[1]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                    case "-t":
                    case "--test":
                        Console.WriteLine("Printing test png called: test.png ");
                        Console.WriteLine(DotToPng.CreatePngFile() ? "print succeeded!" : "print failed!");
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

        public static bool TryParse(string filePath, out Parser parser)
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
            return true;
        }
    }
}
