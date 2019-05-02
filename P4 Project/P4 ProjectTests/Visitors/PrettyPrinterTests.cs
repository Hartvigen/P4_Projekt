using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Visitors;
using P4_ProjectTests1.Visitors.TestCode;

namespace P4_ProjectTests1.Visitors
{
    [TestFixture]
    public class PrettyPrinterTests
    {
        private static Parser _parserWithUglyCode;
        private static Scanner _scannerUgly;

        private static Parser _parserWithPrettyCode;
        private static Scanner _scannerPretty;
        
        [OneTimeSetUp]
        public static void ClassInit()
        {
        }

        [SetUp]
        public void Initialize()
        {
            _scannerPretty = new Scanner(StreamFromString(KnownGoodFiles.PrettyCode));
            _scannerUgly = new Scanner(StreamFromString(KnownGoodFiles.UglyCode));

            _parserWithUglyCode = new Parser(_scannerUgly);
            _parserWithPrettyCode = new Parser(_scannerPretty);
        }

        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        [TearDown]
        public void Cleanup()
        {
            _parserWithUglyCode = null;
            _parserWithPrettyCode = null;
            _scannerPretty = null;
            _scannerUgly = null;
        }

        private static string Prettify(string program)
        {
            var parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            var prettyPrinter = new PrettyPrinter();
            parser.mainNode.Accept(prettyPrinter);
            return prettyPrinter.Result.ToString();
        }

        //Making Pretty written code pretty actually makes no changes 
        [Test]
        public void PrettyPrinterTest01()
        {
            var program = Prettify(KnownGoodFiles.PrettyCode);
            var startProgram = KnownGoodFiles.PrettyCode;

            Assert.IsTrue(program == startProgram);
        }

        //Prettifying code 10 times changes nothing to the actual code. 
        [Test]
        public void PrettyPrinterTest02()
        {
            var program = KnownGoodFiles.PrettyCode;

            for (var i = 10; i > 0; i--)
                program = Prettify(program);

            Assert.IsTrue(program == KnownGoodFiles.PrettyCode);
        }

        //Prettifying ugly code makes the code actually different.
        [Test]
        public void PrettyPrinterTest03()
        {
            var program = KnownGoodFiles.UglyCode;
            Assert.IsFalse(program == KnownGoodFiles.PrettyCode);
        }

        //Prettifying ugly code makes the code actually different but subsequent prettifying does nothing..
        [Test]
        public void PrettyPrinterTest04()
        {
            string program = KnownGoodFiles.UglyCode;
            Assert.IsFalse(Prettify(program) == program);

            program = Prettify(program);

            Assert.IsFalse(Prettify(program) != program);
        }

        //The UglyCode and prettyCode aren't actually the exact same code.
        [Test]
        public void PrettyPrinterTest05()
        {
            Assert.IsFalse(KnownGoodFiles.UglyCode == KnownGoodFiles.PrettyCode);
        }

        //The Ugly Code gets pretty after exactly one prettify
        [Test]
        public void PrettyPrinterTest06()
        {
            Assert.IsTrue(KnownGoodFiles.PrettyCode == Prettify(KnownGoodFiles.UglyCode));
        }

        //The Ugly code parses without error
        [Test]
        public void PrettyPrinterTest07()
        {
            _parserWithUglyCode.Parse();
            Assert.IsTrue(_parserWithUglyCode.errors.count == 0);
        }

        //The pretty code parses without error
        [Test]
        public void PrettyPrinterTest08()
        {
            _parserWithPrettyCode.Parse();
            Assert.IsTrue(_parserWithPrettyCode.errors.count == 0);
        }

        //This tests the performance of the PrettyPrinter, it should complete the 1000 prettifies in under 1 second if not there is probably 
        //something expensive going on depending on the speed of you computer it might be actually be okay.
        [Test]
        public void PrettyPrinterTest09()
        {

            var program = KnownGoodFiles.UglyCode;

            var unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            for (var i = 1000; i > 0; i--)
                Prettify(program);

            var elapsed = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - unixTimestamp;

            Assert.IsTrue(elapsed < 2);
        }
    }
}