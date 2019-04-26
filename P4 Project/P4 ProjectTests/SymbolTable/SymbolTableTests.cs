using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.SymTab.Tests
{
    [TestClass()]
    public class SymbolTableTests
    {
        private static Parser parser;
        private static SymbolTable parent;
        private static SymbolTable symbolTable;

        //Generates a header attribute declaration for vertex and edge where all types are being used.
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            parser = new Parser(null);
            parent = new SymbolTable(null, null);
        }

        [TestInitialize]
        public void Initialize()
        {
            symbolTable = new SymbolTable(parent,parser);
        }

        [TestCleanup]
        public void Cleanup()
        {
            symbolTable = null;
        }

        //As no scopes have been opened we assert there are no scopes.
        [TestMethod()]
        public void SymbolTableTests01()
        {
            Assert.IsTrue(symbolTable.GetScopes().Count == 0);
        }

        //For every time we open a scope another scope should exist in the symboltable.
        [TestMethod()]
        public void SymbolTableTests02()
        {
            int numberOfScopes = 42;
            for(int i = numberOfScopes; i > 0; i--)
            symbolTable.OpenScope();
            Assert.IsTrue(symbolTable.GetScopes().Count == numberOfScopes);
        }

        //Closing the scope will return the parent.
        [TestMethod()]
        public void SymbolTableTests03()
        {
            Assert.IsTrue(symbolTable.CloseScope() == parent);
        }

        //We are just asserting that this test can get to the end without casting exceptions, we can open a butload of scopes and go out of them again
        [TestMethod()]
        public void SymbolTableTests04()
        {
            SymbolTable parent = symbolTable;
            int numberOfScopes = 42;
            for (int i = numberOfScopes; i > 0; i--)
                parent = parent.OpenScope();

            for (int i = numberOfScopes; i > 0; i--)
                parent = parent.CloseScope();
                
            Assert.IsTrue(true);
        }
    }
}