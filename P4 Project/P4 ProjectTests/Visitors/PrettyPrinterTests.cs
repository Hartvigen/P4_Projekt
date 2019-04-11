using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.IO;
using System.Text;

namespace P4_Project.Visitors.Tests
{
    [TestClass()]
    public class PrettyPrinterTests
    {
        static Parser parserWithUglyCode;
        static Scanner scannerUgly;
        static string pathToUglyCode;

        static Parser parserWithPrettyCode;
        static Scanner scannerPretty;
        static string pathToPrettyCode;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            pathToUglyCode = "../../../../P4 Project/P4 ProjectTests/Visitors/TestCode/UglyCode.txt";
            pathToPrettyCode = "../../../../P4 Project/P4 ProjectTests/Visitors/TestCode/PrettyCode.txt";
        }

        [TestInitialize]
        public void Initialize()
        {
            scannerPretty = new Scanner(StreamFromString(File.ReadAllText(pathToPrettyCode)));
            scannerUgly = new Scanner(StreamFromString(File.ReadAllText(pathToPrettyCode)));

            parserWithUglyCode = new Parser(scannerUgly);
            parserWithPrettyCode = new Parser(scannerPretty);
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
        }

        public static string Prettify(string program)
        {
            Parser parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            PrettyPrinter prettyPrinter = new PrettyPrinter();
            parser.mainNode.Accept(prettyPrinter, null);
            return prettyPrinter.str.ToString();
        }

        //Makeing Pretty written code pretty actually makes no changes 
        [TestMethod()]
        public void PrettyPrinterTest01()
        {
            string program = Prettify(File.ReadAllText(pathToPrettyCode));
            string startprogram = File.ReadAllText(pathToPrettyCode);

            Assert.IsTrue(program == startprogram);
        }

        //Prettyfying code 10 times changes nothing to the actual code. 
        [TestMethod()]
        public void PrettyPrinterTest02()
        {
            string program = File.ReadAllText(pathToPrettyCode);

            for (int i = 10; i > 0; i--)
                program = Prettify(program);

            Assert.IsTrue(program == File.ReadAllText(pathToPrettyCode));
        }

        //Prettyfying ugly code makes the code actually different.
        [TestMethod()]
        public void PrettyPrinterTest03()
        {
            string program = File.ReadAllText(pathToUglyCode);
            Assert.IsFalse(program == File.ReadAllText(pathToPrettyCode));
        }

        //Prettyfying ugly code makes the code actually different but subsequent prettyfyings does nothing..
        [TestMethod()]
        public void PrettyPrinterTest04()
        {
            string program = File.ReadAllText(pathToUglyCode);
            Assert.IsFalse(Prettify(program) == program);

            program = Prettify(program);

            Assert.IsFalse(Prettify(program) != program);
        }

        //The UglyCode and prettyCode arent actually the exact same code.
        [TestMethod()]
        public void PrettyPrinterTest05()
        {
            Assert.IsFalse(File.ReadAllText(pathToUglyCode) == File.ReadAllText(pathToPrettyCode));
        }

        //The Ugly Code gets pretty after exactly one prettify
        [TestMethod()]
        public void PrettyPrinterTest06()
        {
            Assert.IsTrue(File.ReadAllText(pathToPrettyCode) == Prettify(File.ReadAllText(pathToUglyCode)));
        }

        //The Ugly code parses without error
        [TestMethod()]
        public void PrettyPrinterTest07()
        {
            parserWithUglyCode.Parse();
            Assert.IsTrue(parserWithUglyCode.errors.count == 0);
        }

        //The pretty code parses without error
        [TestMethod()]
        public void PrettyPrinterTest08()
        {
            parserWithPrettyCode.Parse();
            Assert.IsTrue(parserWithPrettyCode.errors.count == 0);
        }

        //This tests the performence of the prettyprinter, it should complete the 1000 prettyfyings in under 1 second if not there is probably 
        //something expensive going on depending on the speed of you computer it might be actually be okay.
        [TestMethod()]
        public void PrettyPrinterTest09()
        {

            string program = File.ReadAllText(pathToUglyCode);

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            for (int i = 1000; i > 0; i--)
                Prettify(program);

            Int32 elapsed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - unixTimestamp;

            Assert.IsTrue(elapsed < 2);
        }
    }
}