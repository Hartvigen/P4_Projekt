using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.Compiler.SyntaxAnalysis.Tests
{
    [TestClass()]
    public class ScannerTests
    {
        private bool TryParse(string program)
        {
            Parser parser
                = new Parser(
                    new Scanner(
                        StreamFromString(program)
                ));
            parser.Parse();

            return parser.errors.count == 0;
        }

        private Scanner scannerFromString(string s)
        {
            return new Scanner(StreamFromString(s));
        }



        private MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        
        //grammatical error should give error
        [TestMethod()]
        public void ScanTest()
        {
            bool success = true;
            success = TryParse("[vertix()]");
            Assert.IsFalse(success);
        }

        //Tests involving tab space and newline
        [TestMethod()]
        public void ScanTest2()
        {
            //we should be able to parse a file with nothing but with tabs, space and newline without causing a compilation error
            bool success = true;

            success = TryParse(" " + Environment.NewLine + "	");
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
        [TestMethod()]
        public void ScanTest3()
        {
            Assert.IsTrue(scannerFromString("a2").Scan().kind == Parser._IDENT);

            Assert.IsFalse(scannerFromString(")a2").Scan().kind == Parser._IDENT);

            Scanner test = scannerFromString("a2)");
            Assert.IsTrue(test.Scan().kind == Parser._IDENT);
            test.Scan();
            Assert.IsTrue(test.Scan().kind == Parser._EOF);
        }

        //test to see if we can correctly recognize token values
        [TestMethod()]
        public void ScanTest4()
        {
            Assert.IsTrue(scannerFromString("a2").Scan().val == "a2");

            Assert.IsTrue(scannerFromString("a2$").Scan().val == "a2");

            Assert.IsTrue(scannerFromString("2k k").Scan().val == "2");

            Assert.IsTrue(scannerFromString("()").Scan().val == "(");

            Assert.IsTrue(scannerFromString("vertex fur").Scan().val == "vertex");

            Assert.IsTrue(scannerFromString("edge ry").Scan().val == "edge");

            Assert.IsTrue(scannerFromString("number x = 10").Scan().val == "number");

            Assert.IsTrue(scannerFromString("bool flag = true").Scan().val == "bool");
        }

        //test to see if we find the correct token values when scanning through a header
        [TestMethod()]
        public void ScanTest5()
        {
            Assert.IsTrue(TryParse("[vertex(number x = 10)]"));

            Scanner test = scannerFromString("[vertex(number x = 10)]");
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