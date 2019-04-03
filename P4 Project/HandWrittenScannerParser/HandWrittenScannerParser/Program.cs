using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandWrittenScannerParser
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Scanner scanner = new Scanner("MagiaText.txt");
            while (scanner.input.hasNext) {
                scanner.NextToken();
                if (scanner.input.hasNext || scanner.Tokens.Count != 0)
                {
                    Token current = scanner.DeQueueNext();
                    Console.WriteLine($"Kind is : {current.Getkind().ToString()}, Value is : {current.GetValue()}");
                }
            }*/
            Parser parser = new Parser("MagiaText.txt");
            parser.parse();
            Console.ReadKey();
        }
    }
}
