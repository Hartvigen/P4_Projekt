using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Compiler.SemanticAnalysis.Visitors.Extra;
using P4_Project.Compiler.SyntaxAnalysis;

namespace P4_ProjectTests.Visitors
{
    [TestFixture]
    public class XmlTreeBuilderTests
    {
        private static string _uglyProgramPath;
        private static string _prettyProgramPath;

        [OneTimeSetUp]
        public static void ClassInit()
        {
        }

        [SetUp]
        public void Initialize()
        {
            _uglyProgramPath = TestContext.CurrentContext.TestDirectory + "/../../Visitors/TestCode/UglyCode.txt";
            _prettyProgramPath = TestContext.CurrentContext.TestDirectory + "/../../Visitors/TestCode/PrettyCode.txt";
        }

        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        [TearDown]
        public void Cleanup()
        {
        }

        private static string ReturnXmlTree(string program)
        {
            var parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            var xmlTreeBuilder = new XmlTreeBuilder();
            parser.mainNode.Accept(xmlTreeBuilder);
            return xmlTreeBuilder.Result.ToString();
        }

        //Xml Code and pretty MAGIA code is not the same
        [Test]
        public void XmlTreeBuilderTest01()
        {
            var xml = ReturnXmlTree(File.ReadAllText(_prettyProgramPath));
            Assert.IsTrue(xml != File.ReadAllText(_prettyProgramPath));
        }

        //Xml Code and ugly MAGIA code is not the same
        [Test]
        public void XmlTreeBuilderTest02()
        {
            var xml = ReturnXmlTree(File.ReadAllText(_uglyProgramPath));
            Assert.IsTrue(xml != File.ReadAllText(_uglyProgramPath));
        }

        //Xml is not valid MAGIA code at all!
        [Test]
        public void XmlTreeBuilderTest03()
        {
            var xml = ReturnXmlTree(File.ReadAllText(_uglyProgramPath));
            var parser = new Parser(new Scanner(StreamFromString(xml)));
            parser.Parse();
            Assert.IsTrue(parser.errors.count > 0);
        }

        //Xml Build from ugly code is equal to Xml build from pretty code
        [Test]
        public void XmlTreeBuilderTest04()
        {
            var xmlPretty = ReturnXmlTree(File.ReadAllText(_prettyProgramPath));
            var xmlUgly = ReturnXmlTree(File.ReadAllText(_uglyProgramPath));
            Assert.IsTrue(xmlPretty == xmlUgly);
        }

        //The Xml contains an even number of lines (There must allays be a close tag for every being tag and each are on their own line) 
        [Test]
        public void XmlTreeBuilderTest05()
        {
            string xml = ReturnXmlTree(File.ReadAllText(_prettyProgramPath));
            Assert.IsTrue((xml.Length - xml.Replace(Environment.NewLine, string.Empty).Length) % 2 == 0);
        }

        //This tests the performance of the XmlTreeBuilder, it should complete the 1000 Builds in under 1 second if not there is probably 
        //something expensive going on depending on the speed of you computer it might be actually be okay.
        [Test]
        public void XmlTreeBuilderTest06()
        {

            string program = File.ReadAllText(_prettyProgramPath);

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            for (int i = 1000; i > 0; i--)
                ReturnXmlTree(program);

            Int32 elapsed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - unixTimestamp;

            Assert.IsTrue(elapsed < 2);
        }
    }
}