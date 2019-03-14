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

        
        [TestMethod()]
        public void ScanTest()
        {
            bool success = true;
            success = TryParse("[vertix()]");
            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ScanTest2()
        {
            bool success;
            success = TryParse("    \n");
            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ScanTest3()
        {
            bool success;

        }
    }
}