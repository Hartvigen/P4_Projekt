using Microsoft.VisualStudio.TestTools.UnitTesting;
using HandWrittenScannerParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandWrittenScannerParser.Tests
{
    [TestClass()]
    public class ParserTests
    {
        //Tests that CurrentTerminal loads correctly when initializing the parser.
        [TestMethod()]
        public void CurrentTerminalTest()
        {
            Parser parser = new Parser("../../ParserTests/AcceptTest.txt");
            Assert.IsTrue(parser.CurrentTerminal.Getkind().Equals((int) Kinds.LBrack) && parser.CurrentTerminal.GetValue().Equals("["));
        }

        //Tests that a new CurrentTerminal is loaded after calling accept()
        [TestMethod()]
        public void AcceptTest1()
        {
            Parser parser = new Parser("../../ParserTests/AcceptTest.txt");
            Token t = parser.CurrentTerminal;
            parser.accept(Kinds.LBrack);
            Assert.AreNotEqual(t, parser.CurrentTerminal);
        }

        //Tests that accept fails incorrect kinds.
        [TestMethod()]
        public void AcceptTest2()
        {
            Parser parser = new Parser("../../ParserTests/AcceptTest.txt");
            parser.CurrentTerminal = new Token(Kinds.IDENT, "WillFail");
            Assert.ThrowsException<Exception>(() =>parser.accept(Kinds.Func));
        }

        //Tests that ParseHeads throws exceptions if no [ is met first
        [TestMethod()]
        public void parseHead1()
        {
            Parser parser = new Parser("../../ParserTests/ParseHead1.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseHead());
        }

        //Tests that ParseHeads throws exceptions if no Number is met Second
        [TestMethod()]
        public void parseHead2()
        {
            Parser parser = new Parser("../../ParserTests/ParseHead2.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseHead());
        }

        //Tests that ParseHeads throws exceptions if no ] is met lastly
        [TestMethod()]
        public void parseHead3()
        {
            Parser parser = new Parser("../../ParserTests/ParseHead3.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseHead());
        }

        //Tests that ParseHeads accepts a correct head.
        [TestMethod()]
        public void parseHead4()
        {
            Parser parser = new Parser("../../ParserTests/ParseHead4.txt");
            parser.ParseHead();            
        }

        //Tests that ParsteStmts throws an exception if no IDENT is met first
        [TestMethod()]
        public void parseStmtsTest1()
        {
            Parser parser = new Parser("../../ParserTests/ParseStmts1.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseStmts());
        }

        //Tests that ParsteStmts throws an exception if no = is met second
        [TestMethod()]
        public void parseStmtsTest2()
        {
            Parser parser = new Parser("../../ParserTests/ParseStmts2.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseStmts());
        }

        //Tests that ParsteStmts throws an exception if no IDENT is met lastly
        [TestMethod()]
        public void parseStmtsTest3()
        {
            Parser parser = new Parser("../../ParserTests/ParseStmts3.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseStmts());
        }

        //Tests that ParseStmts accepts a correct Stmts body
        [TestMethod()]
        public void parseStmtsTest4()
        {
            Parser parser = new Parser("../../ParserTests/ParseStmts4.txt");
            parser.ParseStmts();
        }

        //Tests that ParseFuncDecl throws an exception if no func is met in the first position.
        [TestMethod()]
        public void parseFuncDecl1()
        {
            Parser parser = new Parser("../../ParserTests/ParseFuncDecl1.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseFuncDecl());
        }

        //Tests that ParseFuncDecl throws an exception if no Ident is met in the 2nd position
        [TestMethod()]
        public void parseFuncDecl2()
        {
            Parser parser = new Parser("../../ParserTests/ParseFuncDecl2.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseFuncDecl());
        }

        //Tests that ParseFuncDecl throws an exception if no ( is met at the 3rd position
        [TestMethod()]
        public void parseFuncDecl3()
        {
            Parser parser = new Parser("../../ParserTests/ParseFuncDecl3.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseFuncDecl());
        }

        //Tests that ParseFuncDecl throws an exception if no ) is met at the 4th position
        [TestMethod()]
        public void parseFuncDecl4()
        {
            Parser parser = new Parser("../../ParserTests/ParseFuncDecl4.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseFuncDecl());
        }

        //Tests that ParseFuncDecl throws an exception if an incorrect Stmts is met lastly
        [TestMethod()]
        public void parseFuncDecl5()
        {
            Parser parser = new Parser("../../ParserTests/ParseFuncDecl5.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseFuncDecl());
        }

        //Tests that ParseFuncDecl parses a FuncDecl correctly 
        [TestMethod()]
        public void parseFuncDecl6()
        {
            Parser parser = new Parser("../../ParserTests/ParseFuncDecl6.txt");
            parser.ParseFuncDecl();
        }

        //Tests that ParseMagia throws an exception if an incorrect header is met firstly
        [TestMethod()]
        public void parseMAGIA1()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest1.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseMAGIA());
        }

        //Tests that ParseMagia throws an exception if an incorrect Stmts is met secondly
        [TestMethod()]
        public void parseMAGIA2()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest2.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseMAGIA());
        }

        //Tests that ParseMagia throws an exception if an incorrect FuncDecl is met lastly
        [TestMethod()]
        public void parseMAGIA3()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest3.txt");
            Assert.ThrowsException<Exception>(() => parser.ParseMAGIA());
        }

        //Tests that ParseMagia parses a small program correctly
        [TestMethod()]
        public void parseMAGIA4()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest4.txt");
            parser.ParseMAGIA();
        }

        //Tests that ParseMagia parses a larger program correctly
        [TestMethod()]
        public void parseMAGIA5()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest5.txt");
            parser.ParseMAGIA();
        }

        //Tests that ParseMagia can skip ParseHead if no such exists
        [TestMethod()]
        public void parseMAGIA6()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest5.txt");
            parser.ParseMAGIA();
        }

        //Tests that ParseMagia Can skip ParseFuncDecl if no such exists
        [TestMethod()]
        public void parseMAGIA7()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest7.txt");
            parser.ParseMAGIA();
        }

        //Tests that ParseMagia can parse only a Stmts and not throw an exception
        [TestMethod()]
        public void parseMAGIA8()
        {
            Parser parser = new Parser("../../ParserTests/ParseTest8.txt");
            parser.ParseMAGIA();
        }

    }
}