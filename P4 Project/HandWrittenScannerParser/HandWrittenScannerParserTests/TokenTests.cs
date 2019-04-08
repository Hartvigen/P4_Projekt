using Microsoft.VisualStudio.TestTools.UnitTesting;
using HandWrittenScannerParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandWrittenScannerParser.Tests
{
    //All of these tests check if the get getValue and GetKind functions work, and if the Kinds enum has the correct value
    [TestClass()]
    public class TokenTests
    {
        [TestMethod()]
        public void GetValueTest1()
        {
            Token t = new Token(Kinds.Number, "IdentNumber_");
            Assert.IsTrue(t.GetValue().Equals("IdentNumber_"));
        }
              
        [TestMethod()]
        public void GetKindTest1()
        {
            Token t = new Token(Kinds.Number, "");
            Assert.IsTrue(t.Getkind().Equals(0));
        }

        [TestMethod()]
        public void GetKindTest2()
        {
            Token t = new Token(Kinds.IDENT, "");
            Assert.IsTrue(t.Getkind().Equals(1));
        }

        [TestMethod()]
        public void GetKindTest3()
        {
            Token t = new Token(Kinds.LBrack, "");
            Assert.IsTrue(t.Getkind().Equals(2));
        }

        [TestMethod()]
        public void GetKindTest4()
        {
            Token t = new Token(Kinds.RBrack, "");
            Assert.IsTrue(t.Getkind().Equals(3));
        }

        [TestMethod()]
        public void GetKindTest5()
        {
            Token t = new Token(Kinds.Assign, "");
            Assert.IsTrue(t.Getkind().Equals(4));
        }

        [TestMethod()]
        public void GetKindTest6()
        {
            Token t = new Token(Kinds.LParen, "");
            Assert.IsTrue(t.Getkind().Equals(5));
        }

        [TestMethod()]
        public void GetKindTest7()
        {
            Token t = new Token(Kinds.RParen, "");
            Assert.IsTrue(t.Getkind().Equals(6));
        }

        [TestMethod()]
        public void GetKindTest8()
        {
            Token t = new Token(Kinds.Func, "");
            Assert.IsTrue(t.Getkind().Equals(7));
        }
    }
}