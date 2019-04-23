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
            //But Dont commit them uncommented as they might destroy everything else.
            
            //Console.WriteLine("Do something custom: ");
            //Console.WriteLine(TryParseAndSymbolCheck(defaultFile) ? "Compile Succeeded!" : "Compile failed!");

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-s":
                    case "--symbolprint":
                        Console.WriteLine("Parsing input file and assigning variables: " + args[1]);
                        Console.WriteLine(TryParseAndSymbolCheck(args[1]) ? "Compile Succeeded!" : "Compile failed!");
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
                        Console.WriteLine(TryParseAndPrettyPrint(args[1]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                    case "-x":
                    case "--xmlprint":
                        Console.WriteLine("Parsing input file and printing XML: " + args[1]);
                        Console.WriteLine(TryParseAndCreateXml(args[1]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                    case "-t":
                    case "--test":
                        Console.WriteLine("Printing test png called: testgraph.png ");
                        Console.WriteLine(DotToPng.createPNGFile() ? "print succeeded!" : "print failed!");
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


        private static bool TryParse(string filePath)
        {
            Parser parser = new Parser(new Scanner(filePath));
            parser.Parse();
            MAGIA AST = parser.mainNode;

            return parser.errors.count == 0;
        }

        private static bool TryParseAndPrettyPrint(string filePath)
        {
            Parser parser = new Parser(new Scanner(filePath));
            parser.Parse();
            PrettyPrinter visitor = new PrettyPrinter();
            parser.mainNode.Accept(visitor, null);

            File.WriteAllText("prettyprint.txt", visitor.str.ToString());

            return parser.errors.count == 0;
        }

        private static bool TryParseAndSymbolCheck(string filePath)
        {
            Parser parser = new Parser(new Scanner(filePath));
            parser.Parse();

            MAGIA AST = parser.mainNode;

            TypeVisitor visitor = new TypeVisitor(parser);
            AST.Accept(visitor, null);

            return parser.errors.count == 0;
        }

        private static bool TryParseAndCreateXml(string filePath)
        {
            Parser parser = new Parser(new Scanner(filePath));
            parser.Parse();

            MAGIA AST = parser.mainNode;

            XmlTreeBuilder visitor = new XmlTreeBuilder();
            AST.Accept(visitor, null);

            File.WriteAllText("xmltree.xml", visitor.ast.ToString());

            return parser.errors.count == 0;
        }
    }
}
