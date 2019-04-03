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
            string text1 = "Hello";
            string nottest = "1Hello";
            Regex rgx = new Regex(@"^[A-Za-z][A-Za-z0-9]*$");
            if(rgx.IsMatch(text1))
                Console.WriteLine("Did it");
            if (!rgx.IsMatch(nottest))
                Console.WriteLine("Didn't do it");
            String path = "MagiaText.txt";
            if (File.Exists(path))
            {
                string program = File.ReadAllText(path, Encoding.UTF8);
                
            
            }

            Console.ReadKey();
        }
    }
}
