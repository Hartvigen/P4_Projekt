using NUnit.Framework;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_ProjectTests1.Visitors
{
    class ScopeTests
    {
        //this class has the purpose of testing the functionality of the ScopeChecker class

        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        private static ScopeChecker Scop(string program)
        {
            var parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            var scopChecker = new ScopeChecker(parser.tab);

            parser.mainNode.Accept(scopChecker);

            return scopChecker;
        }

        //if a variable has been previously declared, accessing it should not cause any errors.
        //we also check for the type of the variable as a double check, though this test is already done in the in the SymbolTableTest
        [Test]
        public void ScopeTestSuccess01()
        {
            var program = Scop("number x = 5 " +
                                "x = 3");

            Assert.IsTrue(program.ErrorList.Count == 0);
            Assert.IsTrue(program.Table.Find("x").type.name.Equals("number"));
        }

        //We should not be able to access a variable that has not been previously declared
        [Test]
        public void ScopeTestFailure01()
        {
            var program = Scop("x = 5");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }


    }
}
