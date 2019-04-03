using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.AST;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.Visitors.Tests
{
    [TestClass()]
    public class PrettyPrinterTests
    {
        static PrettyPrinter visitor;

        static Parser parserWithUglyCode;
        static Scanner scannerUgly;
        static string pathToUglyCode;

        static Parser parserWithPrettyCode;
        static Scanner scannerPretty;
        static string pathToPrettyCode;

        //Generates a header attribute declaration for vertex and edge where all types are being used.
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            pathToUglyCode =   "../../../../P4 Project/P4 ProjectTests/Visitors/UglyCode.txt";
            pathToPrettyCode = "../../../../P4 Project/P4 ProjectTests/Visitors/PrettyCode.txt";
        }

        [TestInitialize]
        public void Initialize()
        {
            scannerPretty = new Scanner(StreamFromString(File.ReadAllText(pathToPrettyCode)));
            scannerUgly = new Scanner(StreamFromString(File.ReadAllText(pathToPrettyCode)));

            parserWithUglyCode = new Parser(scannerUgly);
            parserWithPrettyCode = new Parser(scannerPretty);

            visitor = new PrettyPrinter();
        }

        static private MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        [TestCleanup]
        public void Cleanup()
        {
            parserWithUglyCode = null;
            parserWithPrettyCode = null;
            scannerPretty = null;
            scannerUgly = null;
            visitor = null;
        }

        public static string Prettify(string program)
        {
            Parser parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            PrettyPrinter prettyPrinter = new PrettyPrinter();
            parser.mainNode.Accept(prettyPrinter);
            return prettyPrinter.str.ToString();
        }

        //Makeing Pretty written code pretty actually makes no changes 
        [TestMethod()]
        public void PrettyPrinterTestSuccess01()
        {
            string program = Prettify(File.ReadAllText(pathToPrettyCode));
            string startprogram = File.ReadAllText(pathToPrettyCode);

            Assert.IsTrue(program == startprogram);
        }

        //Prettyfying code 10 times changes nothing to the actual code. 
        [TestMethod()]
        public void PrettyPrinterTestSuccess02()
        {
            string program = File.ReadAllText(pathToPrettyCode);

            for (int i = 10; i > 0; i--)
                program = Prettify(program);
            
            Assert.IsTrue(program == File.ReadAllText(pathToPrettyCode));
        }

        //Prettyfying ugly code makes the code actually different.
        [TestMethod()]
        public void PrettyPrinterTestFailure01()
        {
            string program = File.ReadAllText(pathToUglyCode);
            Assert.IsFalse(program == File.ReadAllText(pathToPrettyCode));
        }

        //Prettyfying ugly code makes the code actually different but subsequent prettyfyings does nothing..
        [TestMethod()]
        public void PrettyPrinterTestFailure02()
        {
            string program = File.ReadAllText(pathToUglyCode);
            Assert.IsFalse(Prettify(program) == program);

            program = Prettify(program);

            Assert.IsFalse(Prettify(program) != program);
        }

        //The UglyCode and prettyCode arent actually the exact same code.
        [TestMethod()]
        public void PrettyPrinterTestFailure03()
        {
            Assert.IsFalse(File.ReadAllText(pathToUglyCode) == File.ReadAllText(pathToPrettyCode));
        }
    }
}