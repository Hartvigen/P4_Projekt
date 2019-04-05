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
    public class ScannerTests
    {
        //Tests that the digit regex can recognize all the correct chars.
        [TestMethod()]
        public void DigitRegexTest1()
        {            
            foreach (char c in "0123456789876543210")
                Assert.IsTrue(Scanner.digitRegex.IsMatch(c.ToString()));
        }

        //Tests that the digit regex does not recognize letters in the middle of a string.
        [TestMethod()]
        public void DigitRegexTest2()
        {
            foreach (char c in "0123456789adv876543210")            
                if (!Scanner.digitRegex.IsMatch(c.ToString()))
                    return;
            
            Assert.Fail();
        }

        //Tests that the digit regex does not not recognize a string of letter chars.
        [TestMethod()]
        public void DigitRegexTest3()
        {
            foreach (char c in "DigitRegexTest")
                Assert.IsFalse(Scanner.digitRegex.IsMatch(c.ToString()));
        }

        //Tests that the IdentFunc Regex recognizes letters, underscore and digits.
        [TestMethod()]
        public void IdentFuncRegexTest1()
        {
            Assert.IsTrue(Scanner.IdentFuncRegex.IsMatch("Hello_123"));
        }

        //Tests that the Number regex recognizes decimal numbers
        [TestMethod()]
        public void NumberRegexTest1()
        {
            foreach (char c in "0123456789.9876543210")
                Assert.IsTrue(Scanner.numberRegex.IsMatch(c.ToString()));
        }

        //Tests that the number regex will recognize numbers with multiple dots, even if this will throw 
        // an exception when making the token.
        [TestMethod()]
        public void NumberRegexTest2()
        {
            foreach (char c in "01234.56789.98765.43210")
                Assert.IsTrue(Scanner.numberRegex.IsMatch(c.ToString()));
        }

        //Tests that letter chars cannot be recognized by number regex in a decimal number.
        [TestMethod()]
        public void NumberRegexTest3()
        {
            foreach (char c in "0123456789.5hello1234")
                if (!Scanner.numberRegex.IsMatch(c.ToString()))
                    return;

            Assert.Fail();
        }
        

        //Tests that the enqueuement and dequeuement of tokens work correctly.
        [TestMethod()]
        public void DeQueueNextTest()
        {
            Scanner scanner = new Scanner("../../ScannerTests/ScannerTest1.txt");
            Token t1 = new Token(Kinds.IDENT, "1");
            Token t2 = new Token(Kinds.Number, "2");
            scanner.Tokens.Enqueue(t1);
            Assert.IsTrue(scanner.DeQueueNext().Equals(t1));
        }

        //Tests if [ tokens are made correctly
        [TestMethod()]
        public void NextTokenTest1()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken1.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.LBrack) && t.GetValue().Equals("["));
        }

        //Test if ] tokens are made correctly
        [TestMethod()]
        public void NextTokenTest2()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken2.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.RBrack) && t.GetValue().Equals("]"));
        }

        //Test if = tokens are made correctly
        [TestMethod()]
        public void NextTokenTest3()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken3.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.Assign) && t.GetValue().Equals("="));

        }

        //Test if ( tokens are made correctly
        [TestMethod()]
        public void NextTokenTest4()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken4.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.LParen) && t.GetValue().Equals("("));
        }

        //Test if ) tokens are made correctly
        [TestMethod()]
        public void NextTokenTest5()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken5.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.RParen) && t.GetValue().Equals(")"));
        }

        //Test that no token is made when an unrecognized char appears at the start of the next token
        [TestMethod()]
        public void NextTokenTest6()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken6.txt");
            Assert.ThrowsException<Exception>(() => scanner.NextToken());            
        }

        //Test if IDENT tokens are made
        [TestMethod()]
        public void NextTokenTest7()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken7.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals("Ident"));
        }

        //Test if func tokens are made correctly
        [TestMethod()]
        public void NextTokenTest8()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken8.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.Func) && t.GetValue().Equals("func"));
        }

        //Test if no tokens are made from an ident that starts with a number.
        [TestMethod()]
        public void NextTokenTest9()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken9.txt");
                        Assert.ThrowsException<Exception>(() => scanner.NextToken());
        }

        //Test if Number tokens are made correctly
        [TestMethod()]
        public void NextTokenTest10()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken10.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.Number) && t.GetValue().Equals("12345"));
        }

        //Test if Whitespace is properly skipped
        [TestMethod()]
        public void NextTokenTest11()
        {
            Scanner scanner = new Scanner("../../ScannerTests/NextToken/NextToken11.txt");
            scanner.NextToken();
            Token t = scanner.DeQueueNext();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals("MAGIA"));
        }

        //Test if a number token is made from a integer
        [TestMethod()]
        public void NumberTokenTest1()
        {
            Scanner scanner = new Scanner("../../ScannerTests/ScannerTest1.txt");
            Assert.IsTrue(scanner.NumberToken().GetValue().Equals("123"));
        }

        //Test if a number token is made from a decimal 
        [TestMethod()]
        public void NumberTokenTest2()
        {
            Scanner scanner = new Scanner("../../ScannerTests/ScannerTest2.txt");
            Assert.IsTrue(scanner.NumberToken().GetValue().Equals("123.456"));
        }

        //Test that no token is made from a number with multiple decimal dots.
        [TestMethod()]
        public void NumberTokenTest3()
        {
            Scanner scanner = new Scanner("../../ScannerTests/ScannerTest3.txt");
            Assert.ThrowsException<Exception>(() => scanner.NumberToken());
        }

        //Tests that letters aren't included.
        [TestMethod()]
        public void NumberTokenTest4()
        {
            Scanner scanner = new Scanner("../../ScannerTests/ScannerTest4.txt");
            Assert.IsTrue(scanner.NumberToken().GetValue().Equals(""));
        }

        //Tests that all possible chars are recognized for the token.
        [TestMethod()]
        public void IdentOrFuncTokenTest1()
        {
            Scanner scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc1.txt");            
            Token t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789"));
        }

        //Test that both digits and letters are recognized.
        [TestMethod()]
        public void IdentOrFuncTokenTest2()
        {
            Scanner scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc2.txt");
            Token t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals("1Ident"));
        }

        //Test that digits and letters can alternate between each other.
        [TestMethod()]
        public void IdentOrFuncTokenTest3()
        {
            Scanner scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc3.txt");
            Token t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals("Ident2funcb4EOF"));
        }

        //Test that ident tokens are made correctly, and stop at White space
        [TestMethod()]
        public void IdentOrFuncTokenTest4()
        {
            Scanner scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc4.txt");
            Token t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals("Ident"));
        }

        //Tests that unknown chars and whitespace are not included or skipped. 
        //Note that at least one letter or underscore WILL normally exist
        //as the caller is in an IF that requires that.
        [TestMethod()]
        public void IdentOrFuncTokenTest5()
        {
            Scanner scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc51.txt");
            Token t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals(""));
            scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc52.txt");
            t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int)Kinds.IDENT) && t.GetValue().Equals(""));
        }

        //Tests that a func token is made correctly.
        [TestMethod()]
        public void IdentOrFuncTokenTest6()
        {
            Scanner scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc6.txt");
            Token t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int) Kinds.Func) && t.GetValue().Equals("func"));
        }

        //Tests that func tokens arent made when the IDENT is similar
        [TestMethod()]
        public void IdentOrFuncTokenTest7()
        {
            Scanner scanner = new Scanner("../../ScannerTests/IdentFunc/IdentFunc7.txt");
            Token t = scanner.IdentOrFuncToken();
            Assert.IsTrue(t.Getkind().Equals((int) Kinds.IDENT) && t.GetValue().Equals("function"));
        }
    }
}