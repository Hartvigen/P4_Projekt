using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using P4_Project.Graphviz;

namespace P4_ProjectTests.Graphviz
{
       [TestFixture]
    public class DotToPngTests
    {
        private static readonly string CustomFilePath = AppDomain.CurrentDomain.BaseDirectory + "test.png";
        private const string CustomDotCode = "digraph{one -> three; two -> four; three -> five; four -> six;}";

        [OneTimeSetUp]
        public static void ClassInit()
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            if (xmlDoc.DocumentElement != null)
                foreach (XmlElement element in xmlDoc.DocumentElement)
                {
                    if (!element.Name.Equals("appSettings")) continue;
                    foreach (XmlNode node in element.ChildNodes)
                    {
                        if (node.Attributes != null && node.Attributes[0].Value.Equals("graphVizLocation"))
                        {
                            node.Attributes[1].Value =
                                TestContext.CurrentContext.TestDirectory + "\\..\\..\\Graphviz\\bin";
                        }
                    }
                }

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            ConfigurationManager.RefreshSection("appSettings");
        }

        [SetUp]
        public void Initialize()
        {
            if (File.Exists(DotOutputGenerator.DefaultFilePath))
                File.Delete(DotOutputGenerator.DefaultFilePath);
            if (File.Exists(CustomFilePath))
                File.Delete(CustomFilePath);
        }

        private static bool IsLinux() {
            return Path.DirectorySeparatorChar == '/';
        }

        [TearDown]
        public void Cleanup()
        {
        }

        private static bool FileEquals(string path1, string path2)
        {
            var file1 = File.ReadAllBytes(path1);
            var file2 = File.ReadAllBytes(path2);
            if (file1.Length != file2.Length) return false;
            return !file1.Where((t, i) => t != file2[i]).Any();
        }

        //The defaults will create an example file PNG file.
        [Test]
        public void DotToPngTestSuccess01()
        {
            if (IsLinux()) return;
            DotOutputGenerator.CreateDefaultPngFile();
            Assert.IsTrue(File.Exists(DotOutputGenerator.DefaultFilePath));
        }

        //The custom DOT code at a custom filepath will create an PNG file at the custom path.
        [Test]
        public void DotToPngTestSuccess02()
        {
            if (IsLinux()) return;
            DotOutputGenerator.CreateCustomPngFile(CustomDotCode, CustomFilePath);
            Assert.IsTrue(File.Exists(CustomFilePath));
        }

        //The custom Code and default code is not the same.
        [Test]
        public void DotToPngTestSuccess03()
        {
            if (IsLinux()) return;
            Assert.IsTrue(CustomDotCode != DotOutputGenerator.DefaultDotCode);
        }

        //The custom path and default path is not the same.
        [Test]
        public void DotToPngTestSuccess04()
        {
            if (IsLinux()) return;
            Assert.IsTrue(CustomFilePath != DotOutputGenerator.DefaultFilePath);
        }

        //The same picture will be generate given the same code.
        [Test]
        public void DotToPngTestSuccess05()
        {
            if (IsLinux()) return;
            //The default picture
            DotOutputGenerator.CreateDefaultPngFile();

            //Should be the same picture just at another filepath
            DotOutputGenerator.CreateCustomPngFile(DotOutputGenerator.DefaultDotCode, CustomFilePath);

            Assert.IsTrue(FileEquals(CustomFilePath, DotOutputGenerator.DefaultFilePath));
        }

        //The pictures are different if different Dot code is given.
        [Test]
        public void DotToPngTestSuccess06()
        {
            if (IsLinux()) return;
            //The default picture
            DotOutputGenerator.CreateDefaultPngFile();

            //Should be the same picture just at another filepath
            DotOutputGenerator.CreateCustomPngFile(CustomDotCode, CustomFilePath);

            Assert.IsTrue(!FileEquals(CustomFilePath, DotOutputGenerator.DefaultFilePath));
        }

        //The pictures are the same if same Dot code is given even if it is custom code.
        [Test]
        public void DotToPngTestSuccess07()
        {
            if (IsLinux()) return;
            //The default picture
            DotOutputGenerator.CreateCustomPngFile(CustomDotCode, DotOutputGenerator.DefaultFilePath);

            //Should be the same picture just at another filepath
            DotOutputGenerator.CreateCustomPngFile(CustomDotCode, CustomFilePath);

            Assert.IsTrue(FileEquals(CustomFilePath, DotOutputGenerator.DefaultFilePath));
        }
    }
}
