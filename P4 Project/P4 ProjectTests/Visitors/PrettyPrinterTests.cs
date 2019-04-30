using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Visitors.Tests.TestCode;

namespace P4_Project.Visitors.Tests
{
    [TestFixture]
    public class PrettyPrinterTests
    {
        static Parser parserWithUglyCode;
        static Scanner scannerUgly;

        static Parser parserWithPrettyCode;
        static Scanner scannerPretty;
        
        [OneTimeSetUp]
        public static void ClassInit()
        {
        }

        [SetUp]
        public void Initialize()
        {
            scannerPretty = new Scanner(StreamFromString(KnownGoodFiles.prettyCode));
            scannerUgly = new Scanner(StreamFromString(KnownGoodFiles.uglyCode));

            parserWithUglyCode = new Parser(scannerUgly);
            parserWithPrettyCode = new Parser(scannerPretty);
        }

        static private MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        [TearDown]
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
            return prettyPrinter.result.ToString();
        }

        //Makeing Pretty written code pretty actually makes no changes 
        [Test]
        public void PrettyPrinterTest01()
        {
            string program = Prettify(KnownGoodFiles.prettyCode);
            string startprogram = KnownGoodFiles.prettyCode;

            Assert.IsTrue(program == startprogram);
        }

        //Prettyfying code 10 times changes nothing to the actual code. 
        [Test]
        public void PrettyPrinterTest02()
        {
            string program = KnownGoodFiles.prettyCode;

            for (int i = 10; i > 0; i--)
                program = Prettify(program);

            Assert.IsTrue(program == KnownGoodFiles.prettyCode);
        }

        //Prettyfying ugly code makes the code actually different.
        [Test]
        public void PrettyPrinterTest03()
        {
            string program = KnownGoodFiles.uglyCode;
            Assert.IsFalse(program == KnownGoodFiles.prettyCode);
        }

        //Prettyfying ugly code makes the code actually different but subsequent prettyfyings does nothing..
        [Test]
        public void PrettyPrinterTest04()
        {
            string program = KnownGoodFiles.uglyCode;
            Assert.IsFalse(Prettify(program) == program);

            program = Prettify(program);

            Assert.IsFalse(Prettify(program) != program);
        }

        //The UglyCode and prettyCode arent actually the exact same code.
        [Test]
        public void PrettyPrinterTest05()
        {
            Assert.IsFalse(KnownGoodFiles.uglyCode == KnownGoodFiles.prettyCode);
        }

        //The Ugly Code gets pretty after exactly one prettify
        [Test]
        public void PrettyPrinterTest06()
        {
            Assert.IsTrue(KnownGoodFiles.prettyCode == Prettify(KnownGoodFiles.uglyCode));
        }

        //The Ugly code parses without error
        [Test]
        public void PrettyPrinterTest07()
        {
            parserWithUglyCode.Parse();
            Assert.IsTrue(parserWithUglyCode.errors.count == 0);
        }

        //The pretty code parses without error
        [Test]
        public void PrettyPrinterTest08()
        {
            parserWithPrettyCode.Parse();
            Assert.IsTrue(parserWithPrettyCode.errors.count == 0);
        }

        //This tests the performence of the prettyprinter, it should complete the 1000 prettyfyings in under 1 second if not there is probably 
        //something expensive going on depending on the speed of you computer it might be actually be okay.
        [Test]
        public void PrettyPrinterTest09()
        {

            string program = KnownGoodFiles.uglyCode;

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            for (int i = 1000; i > 0; i--)
                Prettify(program);

            Int32 elapsed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - unixTimestamp;

            Assert.IsTrue(elapsed < 2);
        }
    }
}