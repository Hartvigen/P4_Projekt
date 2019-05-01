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
using NUnit.Framework;

namespace P4_Project.SymTab.Tests
{
    [TestFixture]
    public class ObjTests
    {
        private static string customName;
        private static BaseType customType;
        private static int customKind;
        private static SymbolTable customSymbolTable;


        private Obj customObj;
        private Obj nullObj;


        [OneTimeSetUp]
        public static void ClassInit()
        {
            customName = "testName";
            customType = new FunctionType(null, null);
            customKind = 404;
        }

        [SetUp]
        public void Initialize()
        {
            customObj = new Obj(customName, customType, customKind);
            nullObj = new Obj();
        }

        [TearDown]
        public void Cleanup()
        {
            customObj = null;
            nullObj = null;
        }

        //The Constructor actually sets excpected Kind
        [Test]
        public void ObjTests01()
        {
            Assert.IsTrue(customObj.Kind == customKind);
        }


        //The Constructor actually sets excpected name
        [Test]
        public void ObjTests02()
        {
            Assert.IsTrue(customObj.Name == customName);
        }

        //The Constructor actually sets excpected name
        [Test]
        public void ObjTests03()
        {
            Assert.IsTrue(customObj.Type.Equals(customType));
        }

        //The Empty Constructor Gives excpected type
        //Note that we have to use .IsNull as the == operator is overridden and cant be used when Type is null.
        [Test]
        public void ObjTests04()
        {
            Assert.IsNull(nullObj.Type);
        }

        //The Empty Constructor Gives excpected kind
        [Test]
        public void ObjTests05()
        {
            Assert.IsTrue(nullObj.Kind == 0);
        }

        //The Empty Constructor Gives excpected name
        [Test]
        public void ObjTests06()
        {
            Assert.IsTrue(nullObj.Name == null);
        }

    }
}