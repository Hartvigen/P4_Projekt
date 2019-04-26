using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.SymTab;
using P4_Project.Types;
using P4_Project.Types.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.SymTab.Tests
{
    [TestClass()]
    public class ObjTests
    {
        private static string customName;
        private static BaseType customType;
        private static int customKind;
        private static SymbolTable customSymbolTable;


        private Obj customObj;
        private Obj nullObj;


        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            customName = "testName";
            customType = new FunctionType(null, null);
            customKind = 404;
            customSymbolTable = new SymbolTable(null, new Parser(null));
        }

        [TestInitialize]
        public void Initialize()
        {
            customObj = new Obj(customName, customType, customKind, customSymbolTable);
            nullObj = new Obj();
        }

        [TestCleanup]
        public void Cleanup()
        {
            customObj = null;
            nullObj = null;
        }

        //The Constructor actually sets excpected Kind
        [TestMethod()]
        public void ObjTests01()
        {
            Assert.IsTrue(customObj.Kind == customKind);
        }

        //The Constructor actually sets excpected Scope
        [TestMethod()]
        public void ObjTests02()
        {
            Assert.IsTrue(customObj.Scope == customSymbolTable);
        }

        //The Constructor actually sets excpected name
        [TestMethod()]
        public void ObjTests03()
        {
            Assert.IsTrue(customObj.Name == customName);
        }

        //The Constructor actually sets excpected name
        [TestMethod()]
        public void ObjTests04()
        {
            Assert.IsTrue(customObj.Type.Equals(customType));
        }

        //The Empty Constructor Gives excpected kind
        [TestMethod()]
        public void ObjTests05()
        {
            Assert.IsTrue(nullObj.Kind == 0);
        }

        //The Empty Constructor Gives excpected name
        [TestMethod()]
        public void ObjTests06()
        {
            Assert.IsTrue(nullObj.Name == null);
        }

        //The Empty Constructor Gives excpected scope
        [TestMethod()]
        public void ObjTests07()
        {
            Assert.IsTrue(nullObj.Scope == null);
        }

        //The Empty Constructor Gives excpected type
        //Note that we have to use .IsNull as the == operator is overridden and cant be used when Type is null.
        [TestMethod()]
        public void ObjTests08()
        {
            Assert.IsNull(nullObj.Type);
        }
    }
}