using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Compiler.SemanticAnalysis.Visitors;
using P4_Project.Compiler.SyntaxAnalysis;

namespace P4_ProjectTests.Visitors
{
    internal class ScopeTests
    {
        //this class has the purpose of testing the functionality of the ScopeChecker class 

        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        private static ScopeChecker Scope(string program)
        {
            var parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            var scopeChecker = new ScopeChecker(parser.tab);

            parser.mainNode.Accept(scopeChecker);

            return scopeChecker;
        }

        //if a variable has been previously declared, accessing it should not cause any errors. 
        //we also check for the type of the variable as a double check, though this test is already done in the in the SymbolTableTest 
        [Test]
        public void ScopeTestSuccess01()
        {
            var program = Scope("number x = 5 " +
                                "x = 3");

            Assert.IsTrue(program.ErrorList.Count == 0);
            Assert.IsTrue(program.Table.FindVar("x").Type.name.Equals("number"));
        }

        //we should be able to access a variable from any legal scope 
        [Test]
        public void ScopeTestSuccess02()
        {
            var program = Scope("number x = 2 " +
                               "x = 3" +
                               "if(true){" +
                               "x = 2}");
  
            Assert.IsTrue(program.ErrorList.Count == 0);
        }

        //We should be able to declare and access a new variable in a scope, despite it already having been declared in a higher scope 
        [Test]
        public void ScopeTestSuccess03()
        {
            var program = Scope("number x = 5" +
                               "if(true){" +
                               "x = 3 " +
                               "boolean x = true " +
                               "x = false}");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsTrue(program.ErrorList.Count == 0);
        }


        //We should not be able to access a variable that has not been previously declared 
        [Test]
        public void ScopeTestFailure01()
        {
            var program = Scope("x = 5");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }

        //We should not be able to access a variable that is out of scope 
        [Test]
        public void ScopeTestFailure02()
        {
            var program = Scope("if(true){" +
                               "number x = 5} " +
                               "x = 3");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }

        //We should not be able to declare a variable with the same name twice in the same scope 
        [Test]
        public void ScopeTestFailure03()
        {
            var program = Scope("number x = 5" +
                               "boolean x = true");

            program.ErrorList.ForEach(Console.WriteLine);
            Assert.IsFalse(program.ErrorList.Count == 0);
        }


    }
}
