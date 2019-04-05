using System;
using System.IO;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P4_Project.Graphviz.Tests
{
    [TestClass()]
    public class DotToPngTests
    {
        private const string customFilePath = "test.png";
        private const string customDotCode = "digraph{one -> three; two -> four; three -> five; four -> six;}";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            if (File.Exists(DotToPng.defaultFilePath))
                File.Delete(DotToPng.defaultFilePath);
            if (File.Exists(customFilePath))
                File.Delete(customFilePath);
        }

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(DotToPng.defaultFilePath))
                File.Delete(DotToPng.defaultFilePath);
            if (File.Exists(customFilePath))
                File.Delete(customFilePath);
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
        [TestMethod()]
        public void DotToPngTestSuccess01()
        {
            string direc = Directory.GetCurrentDirectory();
            DotToPng.createPNGFile();
            Assert.IsTrue(File.Exists(DotToPng.defaultFilePath));
        }

        //The custom DOT code will create an PNG file.
        [TestMethod()]
        public void DotToPngTestSuccess02()
        {
            DotToPng.createPNGFile(customDotCode);
            Assert.IsTrue(File.Exists(DotToPng.defaultFilePath));
        }

        //The custom DOT code at a custom filepath will create an PNG file at the custom path.
        [TestMethod()]
        public void DotToPngTestSuccess03()
        {
            DotToPng.createPNGFile(customDotCode, customFilePath);
            Assert.IsTrue(File.Exists(customFilePath));
        }

        //The custom Code and defualt code is not the same.
        [TestMethod()]
        public void DotToPngTestSuccess04()
        {
            Assert.IsTrue(customDotCode != DotToPng.defaultDotCode);
        }

        //The custom path and defualt path is not the same.
        [TestMethod()]
        public void DotToPngTestSuccess05()
        {
            Assert.IsTrue(customFilePath != DotToPng.defaultFilePath);
        }

        //The same picture will be genereate given the same code.
        [TestMethod()]
        public void DotToPngTestSuccess06()
        {
            //The defualt picture
            DotToPng.createPNGFile();

            //Should be the same picture just at another filepath
            DotToPng.createPNGFile(DotToPng.defaultDotCode, customFilePath);

            Assert.IsTrue(FileEquals(customFilePath, DotToPng.defaultFilePath));
        }

        //The pictures are different if different Dot code is given.
        [TestMethod()]
        public void DotToPngTestSuccess07()
        {
            //The defualt picture
            DotToPng.createPNGFile();

            //Should be the same picture just at another filepath
            DotToPng.createPNGFile(customDotCode, customFilePath);

            Assert.IsTrue(!FileEquals(customFilePath, DotToPng.defaultFilePath));
        }

        //The pictures are the same if same Dot code is given even if it is custom code.
        [TestMethod()]
        public void DotToPngTestSuccess08()
        {
            //The defualt picture
            DotToPng.createPNGFile(customDotCode, DotToPng.defaultFilePath);

            //Should be the same picture just at another filepath
            DotToPng.createPNGFile(customDotCode, customFilePath);

            Assert.IsTrue(FileEquals(customFilePath, DotToPng.defaultFilePath));
        }
    }
}
