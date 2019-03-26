using P4_Project.AST;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Visitors;
using System;
using System.IO;
using System.Xml.Serialization;

namespace P4_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            String defualtFile = "MAGIAFile.txt";

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-h":
                    case "--help":
                        Console.WriteLine("Compile file: MagiaC.exe [filePath]");
                        Console.WriteLine("For help: MagiaC.exe -h");
                        Console.WriteLine("For help: MagiaC.exe --help");
                        Console.WriteLine("PrettyPrint AST: MagiaC.exe -p [filepath]");
                        Console.WriteLine("PrettyPrint AST: MagiaC.exe --prettyprint [filepath]");
                        Console.WriteLine("XmlTree AST: MagiaC.exe -x [filepath]");
                        Console.WriteLine("XmlTree AST: MagiaC.exe --xmlprint [filepath]");
                        Console.WriteLine("If no arguments are given the compiler will look for default file called: \"" + defualtFile + "\" in its directory and compile complie that.");
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
                    default:
                        Console.WriteLine("Parsing input file: " + args[0]);
                        Console.WriteLine(TryParse(args[0]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                }
            }
            else if (File.Exists(defualtFile))
            {
                Console.WriteLine("Compiling standard file: " + defualtFile);
                Console.WriteLine(TryParse(defualtFile) ? "Compile succeeded!" : "Compile failed!");
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
            MAGIA AST = parser.mainNode;

            PrettyPrinter visitor = new PrettyPrinter();
            AST.Accept(visitor);

            File.WriteAllText("prettyprint.txt", visitor.str.ToString());

            return parser.errors.count == 0;
        }

        private static bool TryParseAndCreateXml(string filePath)
        {
            Parser parser = new Parser(new Scanner(filePath));
            parser.Parse();

            MAGIA AST = parser.mainNode;

            XmlTreeBuilder visitor = new XmlTreeBuilder();
            AST.Accept(visitor);

            File.WriteAllText("xmltree.xml", visitor.ast.ToString());

            return parser.errors.count == 0;
        }
    }
}
