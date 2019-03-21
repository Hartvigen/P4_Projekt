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
                        Console.WriteLine("If no arguments are given the compiler will look for default file called: \"" + defualtFile + "\" in its directory and compile complie that.");
                        break;
                    case "-p":
                    case "--prettyprint":
                        Console.WriteLine("Compiling input file and printing PrettyPrinting it: " + args[1]);
                        Console.WriteLine(TryParseAndDebug(args[1]) ? "Compile asucceeded!" : "Compile failed!");
                        break;
                    default:
                        Console.WriteLine("Compiling input file: " + args[0]);
                        Console.WriteLine(TryParse(args[0]) ? "Compile succeeded!" : "Compile failed!");
                        break;
                }
            }
            else if (File.Exists(defualtFile))
            {
                Console.WriteLine("Compiling standard file: " + defualtFile);
                Console.WriteLine(TryParseAndDebug(defualtFile) ? "Compile succeeded!" : "Compile failed!");
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

        private static bool TryParseAndDebug(string filePath)
        {
            Parser parser = new Parser(new Scanner(filePath));
            parser.Parse();
            MAGIA AST = parser.mainNode;

            SerializerVisitor visitor = new SerializerVisitor();
            AST.Accept(visitor);
            string str = visitor.ast.ToString();

            File.WriteAllText("ast.xml", str);

            return parser.errors.count == 0;
        }
    }
}
