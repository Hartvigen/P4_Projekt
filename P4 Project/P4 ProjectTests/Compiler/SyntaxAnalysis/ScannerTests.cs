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
            bool success;
            success = TryParse("    \n");
            Assert.IsTrue(success);

            //a simple program in normal form which should be able to run
            success = TryParse("[vertex(number num = 4, bool val = true)]");
            Assert.IsTrue(success);

            //We should be able to parse the same string with any number of tabs, spaces or newlines. As well as without spacing, except for when declaring types
            success = TryParse("[           vertex       (     number          num      = \n\n   4,  bool     val    =   true    )         ]");
            Assert.IsTrue(success);
            success = TryParse("[vertex(number num=4,bool val=true)]");
            Assert.IsTrue(success);

        }

        //nothing here yet :DDDDDD
        [TestMethod()]
        public void ScanTest3()
        {
            bool success;

        }
    }
}