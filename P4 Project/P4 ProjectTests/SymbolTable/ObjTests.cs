using NUnit.Framework;
using P4_Project.AST;
using P4_Project.SymbolTable;

namespace P4_ProjectTests.SymbolTable
{
    [TestFixture]
    public class ObjTests
    {
        private static string _customName;
        private static BaseType _customType;
        private static int _customKind;

        private Obj _customObj;
        private Obj _nullObj;


        [OneTimeSetUp]
        public static void ClassInit()
        {
            _customName = "testName";
            _customType = new BaseType(null);
            _customKind = 404;
        }

        [SetUp]
        public void Initialize()
        {
            _customObj = new Obj(_customName, _customType, _customKind);
            _nullObj = new Obj();
        }

        [TearDown]
        public void Cleanup()
        {
            _customObj = null;
            _nullObj = null;
        }

        //The Constructor actually sets expected Kind
        [Test]
        public void ObjTests01()
        {
            Assert.IsTrue(_customObj.Kind == _customKind);
        }

        //The Constructor actually sets expected name
        [Test]
        public void ObjTests02()
        {
            Assert.IsTrue(_customObj.Name == _customName);
        }

        //The Constructor actually sets expected name
        [Test]
        public void ObjTests03()
        {
            Assert.IsTrue(_customObj.Type.Equals(_customType));
        }

        //The Empty Constructor Gives expected kind
        [Test]
        public void ObjTests04()
        {
            Assert.IsTrue(_nullObj.Kind == 0);
        }

        //The Empty Constructor Gives expected name
        [Test]
        public void ObjTests05()
        {
            Assert.IsTrue(_nullObj.Name == null);
        }

        //The Empty Constructor Gives expected type
        //Note that we have to use .IsNull as the == operator is overridden and cant be used when Type is null.
        [Test]
        public void ObjTests07()
        {
            Assert.IsNull(_nullObj.Type);
        }
    }
}