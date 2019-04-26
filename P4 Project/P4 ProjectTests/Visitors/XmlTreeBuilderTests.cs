using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.IO;
using System.Text;

namespace P4_Project.Visitors.Tests
{
    [TestClass()]
    public class XmlTreeBuilderTests
    {
        static Parser parserWithUglyCode;
        static Scanner scannerUgly;
        static string pathToUglyCode;

        static Parser parserWithPrettyCode;
        static Scanner scannerPretty;
        static string pathToPrettyCode;

        static string pathToXmlCode;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            pathToUglyCode = "../../../../P4 Project/P4 ProjectTests/Visitors/TestCode/UglyCode.txt";
            pathToPrettyCode = "../../../../P4 Project/P4 ProjectTests/Visitors/TestCode/PrettyCode.txt";
            pathToXmlCode = "../../../../P4 Project/P4 ProjectTests/Visitors/TestCode/xmltree.xml";
        }

        [TestInitialize]
        public void Initialize()
        {
            scannerPretty = new Scanner(StreamFromString(File.ReadAllText(pathToPrettyCode)));
            scannerUgly = new Scanner(StreamFromString(File.ReadAllText(pathToPrettyCode)));

            parserWithUglyCode = new Parser(scannerUgly);
            parserWithPrettyCode = new Parser(scannerPretty);
        }

        static private MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        [TestCleanup]
        public void Cleanup()
        {
            parserWithUglyCode = null;
            parserWithPrettyCode = null;
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
        [TestMethod()]
        public void XmlTreeBuilderTest01()
        {
            string xml = returnXmlTree(File.ReadAllText(pathToPrettyCode));
            Assert.IsTrue(xml != File.ReadAllText(pathToPrettyCode));
        }

        //Xml Code and ugly MAGIA code is not the same
        [TestMethod()]
        public void XmlTreeBuilderTest02()
        {
            string xml = returnXmlTree(File.ReadAllText(pathToUglyCode));
            Assert.IsTrue(xml != File.ReadAllText(pathToUglyCode));
        }

        //Xml is not valid MAGIA code at all!
        [TestMethod()]
        public void XmlTreeBuilderTest03()
        {
            string xml = returnXmlTree(File.ReadAllText(pathToUglyCode));
            Parser parser = new Parser(new Scanner(StreamFromString(xml)));
            parser.Parse();
            Assert.IsTrue(parser.errors.count > 0);
        }

        //Xml Build from ugly code is equal to Xml build from pretty code
        [TestMethod()]
        public void XmlTreeBuilderTest04()
        {
            string xmlpretty = returnXmlTree(File.ReadAllText(pathToPrettyCode));
            string xmlugly = returnXmlTree(File.ReadAllText(pathToUglyCode));
            Assert.IsTrue(xmlpretty == xmlugly);
        }

        //The Xml contains an even number of lines (There must allways be a close tag for every being tag and each are on their own line) 
        [TestMethod()]
        public void XmlTreeBuilderTest05()
        {
            string xml = returnXmlTree(File.ReadAllText(pathToPrettyCode));
            Assert.IsTrue((xml.Length - xml.Replace(Environment.NewLine, string.Empty).Length) % 2 == 0);
        }

        //The Xml pretty generated is equal to known good xml
        [TestMethod()]
        public void XmlTreeBuilderTest06()
        {
            string xml = returnXmlTree(File.ReadAllText(pathToPrettyCode));
            Assert.IsTrue(File.ReadAllText(pathToXmlCode) == xml);
        }

        //The Xml ugly generated is equal to known good xml
        [TestMethod()]
        public void XmlTreeBuilderTest07()
        {
            string xml = returnXmlTree(File.ReadAllText(pathToUglyCode));
            Assert.IsTrue(File.ReadAllText(pathToXmlCode) == xml);
        }

        //This tests the performence of the XmlTreeBuilder, it should complete the 1000 Builds in under 1 second if not there is probably 
        //something expensive going on depending on the speed of you computer it might be actually be okay.
        [TestMethod()]
        public void XmlTreeBuilderTest08()
        {

            string program = File.ReadAllText(pathToPrettyCode);

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            for (int i = 1000; i > 0; i--)
                returnXmlTree(program);

            Int32 elapsed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - unixTimestamp;

            Assert.IsTrue(elapsed < 2);
        }
    }
}