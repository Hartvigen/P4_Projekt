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
    public class InputterTests
    {
        Inputter input;
        
        //Tests whether Current() gets the correct char when called.
        [TestMethod()]
        public void CurrentTest1()
        {
            input = new Inputter("../../InputterTests/InputterTests1.txt");
            char x = input.Current();
            Assert.IsTrue(input.Current().Equals('1'));
        }

        //Tests if every char is read in the correct sequence, and that \n is skipped.
        [TestMethod()]
        public void CurrentTest2()
        {
            input = new Inputter("../../InputterTests/InputterTests1.txt");
            Assert.IsTrue(input.Current().Equals('1'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('2'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('3'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('4'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('5'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('6'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('\r'));
            input.MoveNext();
            Assert.IsFalse(input.Current().Equals('\n'));
            Assert.IsTrue(input.Current().Equals(' '));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('a'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('b'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('d'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('p'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('o'));
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('t'));
        }

        //Tests if, after reaching EOF, Current() will return \n
        [TestMethod()]
        public void CurrentTest3()
        {
            input = new Inputter("../../InputterTests/InputterTests2.txt");
            input.MoveNext();
            Assert.IsTrue(input.Current().Equals('\n'));
        }
        
        //Test if Inputter's own MoveNext() actually changes Current properly, by looking at the enumerator's own Current.
        [TestMethod()]
        public void MoveNextTest1()
        {
            input = new Inputter("../../InputterTests/InputterTests1.txt");
            Assert.IsTrue(input.ProgramString.Current.Equals('1'));
            input.MoveNext();
            Assert.IsTrue(input.ProgramString.Current.Equals('2'));
        }

        //Tests if the inputter's col field is updated after MoveNext().
        [TestMethod()]
        public void MoveNextTest2()
        {
            input = new Inputter("../../InputterTests/InputterTests3.txt");
            Assert.IsTrue(input.col.Equals(1));
            input.MoveNext();
            Assert.IsTrue(input.col.Equals(2));
        }

        //Tests if the Inputter's line field is updated after newline.
        [TestMethod()]
        public void MoveNextTest3()
        {
            input = new Inputter("../../InputterTests/InputterTests3.txt");
            Assert.IsTrue(input.line.Equals(1));
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            Assert.IsTrue(input.line.Equals(2));
        }

        //Tests if col is reset after newline.
        [TestMethod()]
        public void MoveNextTest4()
        {
            input = new Inputter("../../InputterTests/InputterTests3.txt");
            Assert.IsTrue(input.line.Equals(1));
            Assert.IsTrue(input.col.Equals(1));
            input.MoveNext();
            Assert.IsTrue(input.line.Equals(1));
            Assert.IsTrue(input.col.Equals(2));
            input.MoveNext();
            input.MoveNext();
            Assert.IsTrue(input.line.Equals(2));
            Assert.IsTrue(input.col.Equals(1));
        }

        //Tests if col changes after EOF has been reached
        [TestMethod()]
        public void MoveNextTest5()
        {
            input = new Inputter("../../InputterTests/InputterTests2.txt");

            Assert.IsTrue(input.col.Equals(1));
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            input.MoveNext();
            Assert.IsTrue(input.col.Equals(2));
        }

        //Tests if the current char at a certain position is correct. That is, that lines and columns are counted corrently at non-trivial scales.
        [TestMethod()]
        public void InputterTest1()
        {
            input = new Inputter("../../InputterTests/InputterTests4.txt");
            while(input.line < 8 || input.col < 13)
                input.MoveNext();
            Assert.IsTrue(input.Current().Equals('i'));
        }


        //Tests if Inputter reads the EOF position correctly
        [TestMethod()]
        public void InputterTest2()
        {
            input = new Inputter("../../InputterTests/InputterTests4.txt");
            while (input.hasNext)
                input.MoveNext();
            Assert.IsTrue(input.line.Equals(10));
            Assert.IsTrue(input.col.Equals(23));
        }

        //Tests if Inputter reads the end of each line correctly by looking at the current char. Note that the last check, because it is at EOF, is a \n.
        [TestMethod()]
        public void InputterTest3()
        {
            input = new Inputter("../../InputterTests/InputterTests4.txt");
            while (input.hasNext)
            {
                input.MoveNext();
                if(input.line == 1 && input.col == 9){
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 2 && input.col == 11)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 3 && input.col == 13)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 4 && input.col == 1)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 5 && input.col == 13)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 6 && input.col == 1)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 7 && input.col == 22)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 8 && input.col == 25)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 9 && input.col == 26)
                {
                    Assert.IsTrue(input.Current().Equals('\r'));
                }

                if (input.line == 10 && input.col == 23)
                {
                    Assert.IsTrue(input.Current().Equals('\n'));
                }

            }
        }
    }
}