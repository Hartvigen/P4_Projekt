using NUnit.Framework;
using P4_Project.Compiler.SyntaxAnalysis;

namespace P4_ProjectTests1.SymbolTable
{
    [TestFixture]
    public class SymbolTableTests
    {
        private static Parser _parser;
        private static P4_Project.SymbolTable.SymTable _parent;
        private static P4_Project.SymbolTable.SymTable _symTable;

        //Generates a header attribute declaration for vertex and edge where all types are being used.
        [OneTimeSetUp]
        public static void ClassInit()
        {
            _parser = new Parser(null);
            _parent = new P4_Project.SymbolTable.SymTable(null, null);
        }

        [SetUp]
        public void Initialize()
        {
            _symTable = new P4_Project.SymbolTable.SymTable(_parent,_parser);
        }

        [TearDown]
        public void Cleanup()
        {
            _symTable = null;
        }

        //As no scopes have been opened we assert there are no scopes.
        [Test]
        public void SymbolTableTests01()
        {
            Assert.IsTrue(_symTable.GetScopes().Count == 0);
        }

        //For every time we open a scope another scope should exist in the symboltable.
        [Test]
        public void SymbolTableTests02()
        {
            int numberOfScopes = 42;
            for(int i = numberOfScopes; i > 0; i--)
            _symTable.OpenScope();
            Assert.IsTrue(_symTable.GetScopes().Count == numberOfScopes);
        }

        //Closing the scope will return the parent.
        [Test]
        public void SymbolTableTests03()
        {
            Assert.IsTrue(_symTable.CloseScope() == _parent);
        }

        //We are just asserting that this test can get to the end without casting exceptions, we can open a butload of scopes and go out of them again
        [Test]
        public void SymbolTableTests04()
        {
            P4_Project.SymbolTable.SymTable parent = _symTable;
            int numberOfScopes = 42;
            for (int i = numberOfScopes; i > 0; i--)
                parent = parent.OpenScope();

            for (int i = numberOfScopes; i > 0; i--)
                parent = parent.CloseScope();
                
            Assert.IsTrue(true);
        }

        //Functions should be 1 nesting below the global scope
        [Test]
        public void SymbolTableTests05()
        {

        }
    }
}