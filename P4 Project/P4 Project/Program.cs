using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.Collections.Generic;
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
                Console.WriteLine(TryParse(args[0]) ? "Compile succeeded!" : "Compile failed!");
            else
                Console.WriteLine("-- No input file selected. Usage: MagiaC.exe [filePath]");

            Console.ReadKey();
        }



        private static bool TryParse(string filePath)
        {
            Parser parser = new Parser(new Scanner(filePath));
            parser.Parse();

            return parser.errors.count == 0;
        }
    }
}
