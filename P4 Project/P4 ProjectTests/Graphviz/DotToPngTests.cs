using System;
using System.Configuration;
using System.IO;
using System.Linq;
using NUnit.Framework;
using P4_Project.Graphviz;

namespace P4_ProjectTests1.Graphviz
{
    [TestFixture]
    public class DotToPngTests
    {
        private const string CustomFilePath = "test.png";
        private const string CustomDotCode = "digraph{one -> three; two -> four; three -> five; four -> six;}";

        [OneTimeSetUp]
        public static void ClassInit()
        {          
            AppSettingsReader a = new AppSettingsReader();
            Console.Write(a.GetValue("graphVizLocation", typeof(string)));
            if (File.Exists(DotToPng.DefaultFilePath))
                File.Delete(DotToPng.DefaultFilePath);
            if (File.Exists(CustomFilePath))
                File.Delete(CustomFilePath);
        }

        [SetUp]
        public void Initialize()
        {
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(DotToPng.DefaultFilePath))
                File.Delete(DotToPng.DefaultFilePath);
            if (File.Exists(CustomFilePath))
                File.Delete(CustomFilePath);
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
            DotToPng.CreatePngFile();
            Assert.IsTrue(File.Exists(DotToPng.DefaultFilePath));
        }

        //The custom DOT code will create an PNG file.
        [Test]
        public void DotToPngTestSuccess02()
        {
            DotToPng.CreatePngFile(CustomDotCode);
            Assert.IsTrue(File.Exists(DotToPng.DefaultFilePath));
        }

        //The custom DOT code at a custom filepath will create an PNG file at the custom path.
        [Test]
        public void DotToPngTestSuccess03()
        {
            DotToPng.CreatePngFile(CustomDotCode, CustomFilePath);
            Assert.IsTrue(File.Exists(CustomFilePath));
        }

        //The custom Code and default code is not the same.
        [Test]
        public void DotToPngTestSuccess04()
        {
            Assert.IsTrue(CustomDotCode != DotToPng.DefaultDotCode);
        }

        //The custom path and default path is not the same.
        [Test]
        public void DotToPngTestSuccess05()
        {
            Assert.IsTrue(CustomFilePath != DotToPng.DefaultFilePath);
        }

        //The same picture will be generate given the same code.
        [Test]
        public void DotToPngTestSuccess06()
        {
            //The default picture
            DotToPng.CreatePngFile();

            //Should be the same picture just at another filepath
            DotToPng.CreatePngFile(DotToPng.DefaultDotCode, CustomFilePath);

            Assert.IsTrue(FileEquals(CustomFilePath, DotToPng.DefaultFilePath));
        }

        //The pictures are different if different Dot code is given.
        [Test]
        public void DotToPngTestSuccess07()
        {
            //The default picture
            DotToPng.CreatePngFile();

            //Should be the same picture just at another filepath
            DotToPng.CreatePngFile(CustomDotCode, CustomFilePath);

            Assert.IsTrue(!FileEquals(CustomFilePath, DotToPng.DefaultFilePath));
        }

        //The pictures are the same if same Dot code is given even if it is custom code.
        [Test]
        public void DotToPngTestSuccess08()
        {
            //The default picture
            DotToPng.CreatePngFile(CustomDotCode);

            //Should be the same picture just at another filepath
            DotToPng.CreatePngFile(CustomDotCode, CustomFilePath);

            Assert.IsTrue(FileEquals(CustomFilePath, DotToPng.DefaultFilePath));
        }
    }
}
