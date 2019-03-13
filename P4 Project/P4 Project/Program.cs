using P4_Project.AST;
using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Compiling input file: " + args[0]);
                Console.WriteLine(TryParse(args[0]) ? "Compile succeeded!" : "Compile failed!");
            }
            else if (File.Exists("MAGIAFile.txt"))
            {
                Console.WriteLine("Compiling standard file: MAGIAFile.txt");
                Console.WriteLine(TryParse("MAGIAFile.txt") ? "Compile succeeded!" : "Compile failed!");
            }
            else
            {
                Console.WriteLine("-- No input file selected. Usage: MagiaC.exe [filePath]");
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
    }
}
