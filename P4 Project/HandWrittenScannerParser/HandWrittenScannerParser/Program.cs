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
            Parser parser = new Parser("MagiaText.txt");
            try { parser.ParseMAGIA(); }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
