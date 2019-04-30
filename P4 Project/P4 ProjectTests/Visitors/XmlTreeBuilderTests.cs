using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Visitors.Tests.TestCode;

namespace P4_Project.Visitors.Tests
{
    [TestFixture]
    public class XmlTreeBuilderTests
    {
        static Scanner scannerUgly;

        static Scanner scannerPretty;

        [OneTimeSetUp]
        public static void ClassInit()
        {
        }

        [SetUp]
        public void Initialize()
        {
            scannerPretty = new Scanner(StreamFromString(KnownGoodFiles.prettyCode));
            scannerUgly = new Scanner(StreamFromString(KnownGoodFiles.uglyCode));
        }

        static private MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        [TearDown]
        public void Cleanup()
        {
            scannerPretty = null;
            scannerUgly = null;
        }

        public static string returnXmlTree(string program)
        {
            Parser parser = new Parser(new Scanner(StreamFromString(program)));
            parser.Parse();
            XmlTreeBuilder xmlTreeBuilder = new XmlTreeBuilder();
            parser.mainNode.Accept(xmlTreeBuilder, null);
            return xmlTreeBuilder.result.ToString();
        }

        //Xml Code and pretty MAGIA code is not the same
        [Test]
        public void XmlTreeBuilderTest01()
        {
            string xml = returnXmlTree(KnownGoodFiles.prettyCode);
            Assert.IsTrue(xml != KnownGoodFiles.prettyCode);
        }

        //Xml Code and ugly MAGIA code is not the same
        [Test]
        public void XmlTreeBuilderTest02()
        {
            string xml = returnXmlTree(KnownGoodFiles.uglyCode);
            Assert.IsTrue(xml != KnownGoodFiles.uglyCode);
        }

        //Xml is not valid MAGIA code at all!
        [Test]
        public void XmlTreeBuilderTest03()
        {
            string xml = returnXmlTree(KnownGoodFiles.uglyCode);
            Parser parser = new Parser(new Scanner(StreamFromString(xml)));
            parser.Parse();
            Assert.IsTrue(parser.errors.count > 0);
        }

        //Xml Build from ugly code is equal to Xml build from pretty code
        [Test]
        public void XmlTreeBuilderTest04()
        {
            string xmlpretty = returnXmlTree(KnownGoodFiles.prettyCode);
            string xmlugly = returnXmlTree(KnownGoodFiles.uglyCode);
            Assert.IsTrue(xmlpretty == xmlugly);
        }

        //The Xml contains an even number of lines (There must allways be a close tag for every being tag and each are on their own line) 
        [Test]
        public void XmlTreeBuilderTest05()
        {
            string xml = returnXmlTree(KnownGoodFiles.prettyCode);
            Assert.IsTrue((xml.Length - xml.Replace(Environment.NewLine, string.Empty).Length) % 2 == 0);
        }

        //The Xml pretty generated is equal to known good xml
        [Test]
        public void XmlTreeBuilderTest06()
        {
            string xml = returnXmlTree(KnownGoodFiles.prettyCode);
            Assert.IsTrue(KnownGoodFiles.xmltree == xml);
        }

        //The Xml ugly generated is equal to known good xml
        [Test]
        public void XmlTreeBuilderTest07()
        {
            string xml = returnXmlTree(KnownGoodFiles.uglyCode);
            Assert.IsTrue(KnownGoodFiles.xmltree == xml);
        }

        //This tests the performence of the XmlTreeBuilder, it should complete the 1000 Builds in under 1 second if not there is probably 
        //something expensive going on depending on the speed of you computer it might be actually be okay.
        [Test]
        public void XmlTreeBuilderTest08()
        {

            string program = KnownGoodFiles.prettyCode;

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            for (int i = 1000; i > 0; i--)
                returnXmlTree(program);

            Int32 elapsed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - unixTimestamp;

            Assert.IsTrue(elapsed < 2);
        }
    }
}