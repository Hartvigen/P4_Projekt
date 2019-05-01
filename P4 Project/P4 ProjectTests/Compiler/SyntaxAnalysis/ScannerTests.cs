using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Compiler.SyntaxAnalysis;

namespace P4_ProjectTests1.Compiler.SyntaxAnalysis
{
    [TestFixture]
    public sealed class ScannerTests
    {
        private static bool TryParse(string program)
        {
            var parser
                = new Parser(
                    new Scanner(
                        StreamFromString(program)
                ));
            parser.Parse();

            return parser.errors.count == 0;
        }

        private Scanner ScannerFromString(string s)
        {
            return new Scanner(StreamFromString(s));
        }

        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }
        
        //grammatical error should give error
        [Test]
        public void ScanTest()
        {
            var success = TryParse("[verity()]");
            Assert.IsFalse(success);
        }

        //Tests involving tab space and newline
        [Test]
        public void ScanTest2()
        {
            //we should be able to parse a file with nothing but with tabs, space and newline without causing a compilation error

            var success = TryParse(" " + Environment.NewLine + "	");
            Assert.IsTrue(success);

            //a simple program in normal form which should be able to run
            success = TryParse("[vertex(number num = 4, boolean val = true)]");
            Assert.IsTrue(success);

            //We should be able to parse the same string with any number of tabs, spaces or newlines. As well as without spacing, except for when declaring types
            success = TryParse("[           vertex       (     number          num      = " + Environment.NewLine + Environment.NewLine + "   4,  boolean     val    =   true    )         ]");
            Assert.IsTrue(success);
            success = TryParse("[vertex(number num=4,boolean val=true)]");
            Assert.IsTrue(success);

        }

        //checking if tokens are read correctly by the scanner
        [Test]
        public void ScanTest3()
        {
            Assert.IsTrue(ScannerFromString("a2").Scan().kind == Parser._IDENT);

            Assert.IsFalse(ScannerFromString(")a2").Scan().kind == Parser._IDENT);

            Scanner test = ScannerFromString("a2)");
            Assert.IsTrue(test.Scan().kind == Parser._IDENT);
            test.Scan();
            Assert.IsTrue(test.Scan().kind == Parser._EOF);
        }

        //test to see if we can correctly recognize token values
        [Test]
        public void ScanTest4()
        {
            Assert.IsTrue(ScannerFromString("a2").Scan().val == "a2");

            Assert.IsTrue(ScannerFromString("a2$").Scan().val == "a2");

            Assert.IsTrue(ScannerFromString("2k k").Scan().val == "2");

            Assert.IsTrue(ScannerFromString("()").Scan().val == "(");

            Assert.IsTrue(ScannerFromString("vertex fur").Scan().val == "vertex");

            Assert.IsTrue(ScannerFromString("edge ry").Scan().val == "edge");

            Assert.IsTrue(ScannerFromString("number x = 10").Scan().val == "number");

            Assert.IsTrue(ScannerFromString("bool flag = true").Scan().val == "bool");
        }

        //test to see if we find the correct token values when scanning through a header
        [Test]
        public void ScanTest5()
        {
            Assert.IsTrue(TryParse("[vertex(number x = 10)]"));

            Scanner test = ScannerFromString("[vertex(number x = 10)]");
            Assert.IsTrue(test.Scan().val == "[");
            Assert.IsTrue(test.Scan().val == "vertex");
            Assert.IsTrue(test.Scan().val == "(");
            Assert.IsTrue(test.Scan().val == "number");
            Assert.IsTrue(test.Scan().val == "x");
            Assert.IsTrue(test.Scan().val == "=");
            Assert.IsTrue(test.Scan().val == "10");
            Assert.IsTrue(test.Scan().val == ")");
            Assert.IsTrue(test.Scan().val == "]");
        }
  
    }
}