using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Compiler.SemanticAnalysis.Visitors;
using P4_Project.Compiler.SemanticAnalysis.Visitors.Extra;
using P4_Project.Compiler.SyntaxAnalysis;

namespace P4_ProjectTests.Visitors
{
    [TestFixture]
    public class PrettyPrinterTests
    {
        private static Parser _parserWithUglyCode;
        private static Scanner _scannerUgly;
        private static string uglyprogrampath;

        private static Parser _parserWithPrettyCode;
        private static Scanner _scannerPretty;
        private static string prettyprogrampath;
        
        [OneTimeSetUp]
        public static void ClassInit()
        {

            uglyprogrampath = TestContext.CurrentContext.TestDirectory + "/../../Visitors/TestCode/UglyCode.txt";
            prettyprogrampath = TestContext.CurrentContext.TestDirectory + "/../../Visitors/TestCode/PrettyCode.txt";
        }

        [SetUp]
        public void Initialize()
        {
        }

        [TearDown]
        public void Cleanup()
        {
            _scannerPretty = null;
            _scannerUgly = null;

            _parserWithUglyCode = null;
            _parserWithPrettyCode = null;
        }

        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        private static string Prettify(string program)
        {
            var parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            var cleaner = new Cleaner(parser.tab);
			var attributeMover = new AttributeMover(parser.tab);
            var scopeChecker = new ScopeChecker(parser.tab);
            var typeVisitor = new TypeChecker(parser.tab);
            var prettyPrinter = new PrettyPrinter();

            parser.mainNode.Accept(cleaner);
			cleaner.ErrorList.ForEach(Console.WriteLine);

			parser.mainNode.Accept(attributeMover);
            attributeMover.ErrorList.ForEach(Console.WriteLine);

			parser.mainNode.Accept(scopeChecker);
            scopeChecker.ErrorList.ForEach(Console.WriteLine);

			parser.mainNode.Accept(typeVisitor);
            typeVisitor.ErrorList.ForEach(Console.WriteLine);
            
            parser.mainNode.Accept(prettyPrinter);
            prettyPrinter.ErrorList.ForEach(Console.WriteLine);

            return prettyPrinter.Result.ToString();
        }

        //Making Pretty written code pretty actually makes no changes 
        [Test]
        public void PrettyPrinterTest01()
        {
            var program = Prettify(File.ReadAllText(prettyprogrampath));
            var startProgram = File.ReadAllText(prettyprogrampath);
            Console.WriteLine(program);
            Assert.IsTrue(program == startProgram);
        }

        //Prettifying code 10 times changes nothing to the actual code. 
        [Test]
        public void PrettyPrinterTest02()
        {
            var program = File.ReadAllText(prettyprogrampath);

            for (var i = 10; i > 0; i--)
                program = Prettify(program);

            Assert.IsTrue(program == File.ReadAllText(prettyprogrampath));
        }

        //Prettifying ugly code makes the code actually different.
        [Test]
        public void PrettyPrinterTest03()
        {
            var program = File.ReadAllText(uglyprogrampath);
            Assert.IsFalse(program == Prettify(File.ReadAllText(uglyprogrampath)));
        }

        //Prettifying ugly code makes the code actually different but subsequent prettifying does nothing..
        [Test]
        public void PrettyPrinterTest04()
        {
            string program = File.ReadAllText(uglyprogrampath);
            Assert.IsFalse(Prettify(program) == program);

            program = Prettify(program);

            Assert.IsFalse(Prettify(program) != program);
        }

        //The UglyCode and prettyCode aren't actually the exact same code.
        [Test]
        public void PrettyPrinterTest05()
        {
            Assert.IsFalse(File.ReadAllText(uglyprogrampath) == File.ReadAllText(prettyprogrampath));
        }

        //The Ugly Code gets pretty after exactly one prettify
        [Test]
        public void PrettyPrinterTest06()
        {
            Assert.IsTrue(File.ReadAllText(prettyprogrampath) == Prettify(File.ReadAllText(uglyprogrampath)));
        }

        //This tests the performance of the PrettyPrinter, it should complete the 1000 prettifies in under 1 second if not there is probably 
        //something expensive going on depending on the speed of you computer it might be actually be okay.
        [Test]
        public void PrettyPrinterTest07()
        {

            var program = File.ReadAllText(prettyprogrampath);

            var unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            for (var i = 1000; i > 0; i--)
                Prettify(program);

            var elapsed = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - unixTimestamp;

            Assert.IsTrue(elapsed < 2);
        }
    }
}