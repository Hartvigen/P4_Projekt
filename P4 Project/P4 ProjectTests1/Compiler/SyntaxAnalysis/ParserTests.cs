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
    public class ParserTests
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
        public void ParseTestSuccess01()
        {
            bool success = TryParse("");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess02()
        {
            bool success = TryParse("[vertex(number num)]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess03()
        {
            bool success = TryParse("[vertex(number num, bool val)]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess04()
        {
            bool success = TryParse("[vertex(number num, bool val, text label)]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess05()
        {
            bool success = TryParse("[vertex(number num, bool val, text label, vertex parent)]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess06()
        {
            bool success = TryParse("[vertex(number num, bool val, text label, vertex parent, edge special)]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess07()
        {
            bool success = TryParse("[vertex(number num)][edge(number num)]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess08()
        {
            bool success = TryParse("[vertex(number num = 50)]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess09()
        {
            bool success = TryParse("[vertex(text str = \"hello\")]");

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void ParseTestSuccess10()
        {
            bool success = TryParse("[vertex(list<text> lst)]");

            Assert.IsTrue(success);
        }





        [TestMethod()]
        public void ParseTestFailure01()
        {
            bool success = TryParse("[]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure02()
        {
            bool success = TryParse("[edge]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure03()
        {
            bool success = TryParse("[vertex]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure04()
        {
            bool success = TryParse("[edge()]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure05()
        {
            bool success = TryParse("[vertex()]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure06()
        {
            bool success = TryParse("[vertex(notatype num)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure07()
        {
            bool success = TryParse("[vertex(number)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure08()
        {
            bool success = TryParse("[edge(number)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure09()
        {
            bool success = TryParse("[edge(number 1notanident)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure10()
        {
            bool success = TryParse("[edge(number 1notanident)] [vertex(number 1notanident)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure11()
        {
            bool success = TryParse("[vertex(number num; bool val)] [vertex(number;)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure12()
        {
            bool success = TryParse("[vertex(list<notatype> lst)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure13()
        {
            bool success = TryParse("[vertex(notatype<text> lst)]");

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void ParseTestFailure14()
        {
            bool success = TryParse("[edge(notatype<text> lst)]");

            Assert.IsFalse(success);
        }
    }
}