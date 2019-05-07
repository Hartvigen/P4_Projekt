using NUnit.Framework;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.Visitors
{
    class CleanerTests
    {
        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        private static Cleaner Clean(string program)
        {
            var parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            var cleaner = new Cleaner(parser.tab);

            parser.mainNode.Accept(cleaner);

            return cleaner;
        }

        //if a variable has been previously declared, accessing it should not cause any errors.
        //we also check for the type of the variable as a double check, though this test is already done in the in the SymbolTableTest
        [Test]
        public void CleanerTestSuccess01()
        {
            var program = Clean("number x = 5 " +
                                "x = 3");

            Assert.IsTrue(program.ErrorList.Count == 0);
            Assert.IsTrue(program.Table.Find("x").type.name.Equals("number"));
        }

        //We should be able to call a declared function
        [Test]
        public void CleanerTestSuccess02()
        {
            var program = Clean("fun() " +
                                "func fun(){}");

            Assert.IsTrue(program.ErrorList.Count == 0);
        }

        //we should be able to call a function if we use the correct parameters
        [Test]
        public void CleanerTestSuccess03()
        {
            var program = Clean("fun(5) " +
                                "func fun(number x){}");
            Assert.IsTrue(program.ErrorList.Count == 0);


            program = Clean("number x = 5 " +
                            "fun(x) " +
                            "func fun(x){}");

            Assert.IsTrue(program.ErrorList.Count == 0);


            program = Clean("boolean b = true " +
                            "number x = 5 " +
                            "fun(x, b) " +
                            "func fun(number num, boolean bol){}");

            Assert.IsTrue(program.ErrorList.Count == 0);
        }

        //functions that do not have return type none, must have atleast one return statement in their body
        [Test]
        public void CleanerTestSuccess04()
        {
            var program = Clean("number x = fun() " +
                                "func number fun(){return 5}");

            Assert.IsTrue(program.ErrorList.Count == 0);
        }

        //Errors should not be thrown if we only have one of each header
        [Test]
        public void CleanerTestSuccess05()
        {
            var program = Clean("[vertex(vertex x, number y)] " +
                                "[edge(edge ed, number y)]");

            Assert.IsTrue(program.ErrorList.Count == 0);
        }

        //We should not be able to access a variable that has not been previously declared
        [Test]
        public void CleanerTestFailure01()
        {
            var program = Clean("x = 5");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }

        //we should not be able to provide parameters for a function which doesn't take any
        [Test]
        public void CleanerTestFailure02()
        {
            var program = Clean("number x = 5 " +
                                "fun(x) " +
                                "func fun(){}");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }

        //We should not be able to call a function with the wrong parameters
        [Test]
        public void CleanerTestFailure03()
        {
            var program = Clean("fun() " +
                                "func fun(number x){}");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }

        //a with a return type that is not "none" must have atleast one return in its expression
        [Test]
        public void CleanerTestFailure04()
        {
            var program = Clean("fun(5) " +
                                "func number fun(number x){}");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }

        //two of the same headers should not be able to exist at once
        [Test]
        public void CleanerTestFailure05()
        {
            var program = Clean("[vertex(number length)] " +
                                "[vertex(number langth)]");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);


            program = Clean("[edge(number length)] " +
                            "[edge(number langth)]");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }
    }
}
