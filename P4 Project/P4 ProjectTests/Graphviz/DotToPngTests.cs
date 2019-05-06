using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using System.Configuration;

namespace P4_Project.Graphviz
{
    [TestFixture]
    public class DotToPngTests
    {
        private static string customFilePath = AppDomain.CurrentDomain.BaseDirectory + "test.png";
        private static string customDotCode = "digraph{one -> three; two -> four; three -> five; four -> six;}";

        [OneTimeSetUp]
        public static void ClassInit()
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            foreach (XmlElement element in xmlDoc.DocumentElement)
            {
                if (element.Name.Equals("appSettings"))
                {
                    foreach (XmlNode node in element.ChildNodes)
                    {
                        if (node.Attributes[0].Value.Equals("graphVizLocation"))
                        {
                            node.Attributes[1].Value = TestContext.CurrentContext.TestDirectory + "\\..\\..\\Graphviz\\bin";
                        }
                    }
                }
            }

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            ConfigurationManager.RefreshSection("appSettings");
        }

        [SetUp]
        public void Initialize()
        {
            if (File.Exists(DotToPng.defaultFilePath))
                File.Delete(DotToPng.defaultFilePath);
            if (File.Exists(customFilePath))
                File.Delete(customFilePath);
        }

        [TearDown]
        public void Cleanup()
        {
        }

        //Taken from https://www.dotnetperls.com/file-equals
        static bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        //The defualts will create an example file PNG file.
        [Test]
        public void DotToPngTestSuccess01()
        {
            DotToPng.CreatePNGFile();
            Assert.IsTrue(File.Exists(DotToPng.defaultFilePath));
        }

        //The custom DOT code at a custom filepath will create an PNG file at the custom path.
        [Test]
        public void DotToPngTestSuccess02()
        {
            DotToPng.CreatePNGFile(customDotCode, customFilePath);
            Assert.IsTrue(File.Exists(customFilePath));
        }

        //The custom Code and defualt code is not the same.
        [Test]
        public void DotToPngTestSuccess03()
        {
            Assert.IsTrue(customDotCode != DotToPng.defaultDotCode);
        }

        //The custom path and defualt path is not the same.
        [Test]
        public void DotToPngTestSuccess04()
        {
            Assert.IsTrue(customFilePath != DotToPng.defaultFilePath);
        }

        //The same picture will be genereate given the same code.
        [Test]
        public void DotToPngTestSuccess05()
        {
            //The defualt picture
            DotToPng.CreatePNGFile();

            //Should be the same picture just at another filepath
            DotToPng.CreatePNGFile(DotToPng.defaultDotCode, customFilePath);

            //Assert.IsTrue(FileEquals(customFilePath, DotToPng.defaultFilePath));
        }

        //The pictures are different if different Dot code is given.
        [Test]
        public void DotToPngTestSuccess06()
        {
            //The defualt picture
            DotToPng.CreatePNGFile();

            //Should be the same picture just at another filepath
            DotToPng.CreatePNGFile(customDotCode, customFilePath);

            //Assert.IsTrue(!FileEquals(customFilePath, DotToPng.defaultFilePath));
        }

        //The pictures are the same if same Dot code is given even if it is custom code.
        [Test]
        public void DotToPngTestSuccess07()
        {
            //The defualt picture
            DotToPng.CreatePNGFile(customDotCode, DotToPng.defaultFilePath);

            //Should be the same picture just at another filepath
            DotToPng.CreatePNGFile(customDotCode, customFilePath);

            //Assert.IsTrue(FileEquals(customFilePath, DotToPng.defaultFilePath));
        }
    }
}
