using P4_Project.AST;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Visitors;
using System;
using System.IO;
using P4_Project.Graphviz;
using P4_Project.Types.Collections;
using P4_Project.Types.Primitives;
using P4_Project.Types;

namespace P4_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string defaultFile = "MAGIAFile.txt";

            //Uncomment these lines if you wanna play with the program            
            Console.WriteLine("Doing custom work: ");
            string[] argss = new string[2];
            args = argss;
            args[0] = "-s";
            args[1] = defaultFile;

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-s":
                    case "--symbolprint":
                        Console.WriteLine("Parsing input file and assigning variables: " + args[1]);
                        tryParse(args[1], out Parser parser);
                        Console.WriteLine(applyVisitor(new TypeVisitor(parser),args[1]) ? "Compile Succeeded!" : "Compile failed!");
                        break;
                    case "-h":
                    case "--help":
                        Console.WriteLine("Compile file: MagiaC.exe [filePath]");
                        Console.WriteLine("For help: MagiaC.exe -h || MagiaC.exe --help");
                        Console.WriteLine("PrettyPrint AST: MagiaC.exe -p [filepath] || MagiaC.exe --prettyprint [filepath]");
                        Console.WriteLine("XmlTree AST: MagiaC.exe -x [filepath] || --xmlprint [filepath]");
                        Console.WriteLine("Create Test Png: MagiaC.exe -t || MagiaC.exe --test");
                        Console.WriteLine("If no arguments are given the compiler will look for default file called: \"" + defaultFile + "\" in its directory and compile complie that.");
                        break;
                    case "-p":
                    case "--prettyprint":
                        Console.WriteLine("Parsing input file and PrettyPrinting: " + args[1]);
                        Console.WriteLine(applyVisitor(new PrettyPrinter(),args[1]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                    case "-x":
                    case "--xmlprint":
                        Console.WriteLine("Parsing input file and printing XML: " + args[1]);
                        Console.WriteLine(applyVisitor(new XmlTreeBuilder(),args[1]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                    case "-t":
                    case "--test":
                        Console.WriteLine("Printing test png called: testgraph.png ");
                        Console.WriteLine(DotToPng.createPNGFile() ? "print succeeded!" : "print failed!");
                        break;
                    default:
                        Console.WriteLine("Parsing input file: " + args[0]);
                        Console.WriteLine(tryParse(args[0], out Parser parser1) ? "Compile succeeded!" : "Compile failed!");
                        break;
                }
            }
            else if (File.Exists(defaultFile))
            {
                Console.WriteLine("Compiling standard file: " + defaultFile);
                Console.WriteLine(tryParse(defaultFile, out Parser parser) ? "Compile succeeded!" : "Compile failed!");
            }
            else
            {
                Console.WriteLine("for help type: MagiaC.exe --help or: MagiaC.exe -h");
            }
            Console.ReadKey();
        }

        private static bool tryParse(string filePath, out Parser parser)
        {
            parser = new Parser(new Scanner(filePath));
            parser.Parse();
            return parser.errors.count == 0;
        }

        private static bool applyVisitor(Visitor visitor, string inputFilePath)
        {
            if (tryParse(inputFilePath, out Parser parser))
            {
                parser.mainNode.Accept(visitor, null);
                if (visitor.errorCount == 0)
                    File.WriteAllText(visitor.appropriateFileName, visitor.result.ToString());
                else return false;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
