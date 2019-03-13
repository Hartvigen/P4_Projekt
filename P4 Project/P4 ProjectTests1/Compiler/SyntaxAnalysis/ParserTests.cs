using Microsoft.VisualStudio.TestTools.UnitTesting;
using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.Compiler.SyntaxAnalysis.Tests
{
    [TestClass()]
    public class ParserTests
    {
        static string validIdentifier = "test123";
        static string validStringType = "text";
        static string invalidIdentifier = "123test";

        static string invalidType = "notAType";
        static string invalidCollection = "notACollection";

        static string headerWithAllTypesVertex;
        static string headerWithAllTypesEdge;


        static List<string> singleTypes = new List<string>()
        {
            "number", "text", "bool", "vertex", "edge"
        };

        static List<string> collectionTypes = new List<string>()
        {
            "list", "queue", "set", "stack"
        };

        //Generates a header attribute declaration for vertex and edge where all types are being used.
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            headerWithAllTypesVertex = "[vertex( ";
            headerWithAllTypesEdge = "[edge( ";

            for (int i = collectionTypes.Count - 1; i >= 0; i--)
            {
                headerWithAllTypesVertex += singleTypes[i] + " " + validIdentifier + ", ";
                for (int j = singleTypes.Count - 1; j >= 0; j--)
                    headerWithAllTypesVertex += (collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + ", ");
            }

            for (int i = collectionTypes.Count - 1; i >= 0; i--)
            {
                headerWithAllTypesEdge += singleTypes[i] + " " + validIdentifier + ", ";
                for (int j = singleTypes.Count - 1; j >= 0; j--)
                    headerWithAllTypesEdge += (collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + ", ");
            }

            headerWithAllTypesEdge = headerWithAllTypesEdge.Remove(headerWithAllTypesEdge.LastIndexOf(","));
            headerWithAllTypesEdge += ")]";

            headerWithAllTypesVertex = headerWithAllTypesVertex.Remove(headerWithAllTypesVertex.LastIndexOf(","));
            headerWithAllTypesVertex += ")]";
        }

        [TestInitialize]
        public void Initialize()
        {

        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        //Will identify an ApproriateValue given a type.
        private string identifyApropriateValue(string type)
        {
            if (collectionTypes.Contains(type))
                switch (type)
                {
                    case "list": return "";
                    case "set": return "";
                    case "stack": return "";
                    case "queue": return "";
                    default: throw new Exception(type + " is not added to the switch but is in the list.");
                }
            else if (singleTypes.Contains(type))
                switch (type)
                {
                    case "number": return "123";
                    case "text": return "\"text\"";
                    case "bool": return "true";
                    case "vertex": return "none";
                    case "edge": return "none";
                    default: throw new Exception(type + " is not added to the switch but is in the list.");
                }
            throw new Exception("Type was neither in type list of collection list.");
        }

        //Will identify an in apropriate value given a type.
        private string identifyInApropriateValue(string type)
        {
            if (collectionTypes.Contains(type))
                switch (type)
                {
                    case "list": return "!";
                    case "set": return "!";
                    case "stack": return "!";
                    case "queue": return "!";
                    default: throw new Exception(type + " is not added to the switch but is in the list of collections.");
                }
            else if (singleTypes.Contains(type))
                switch (type)
                {
                    case "number": return "!";
                    case "text": return "!";
                    case "bool": return "!";
                    case "vertex": return "!";
                    case "edge": return "!";
                    default: throw new Exception(type + " is not added to the switch but is in the list of single types.");
                }
            throw new Exception("Type was neither in type list of collections list.");
        }

        //The actual parser.
        protected bool TryParse(string program)
        {
            Parser parser
                = new Parser(
                    new Scanner(
                        StreamFromString(program)
                ));

            parser.Parse();

            return parser.errors.count == 0;
        }

        private MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        //The Empty String Should be good.  
        [TestMethod()]
        public void ParseTestSuccess01()
        {
            bool success = TryParse("");
            Assert.IsTrue(success);
        }

        //header with valid type and identifier should be good.
        [TestMethod()]
        public void ParseTestSuccess02()
        {
            int i = 0;
            bool success = true;

            for (i = singleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[vertex(" + singleTypes[i] + " " + validIdentifier + ")]");

            for (i = singleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[edge(" + singleTypes[i] + " " + validIdentifier + ")]");

            if (!success)
                Console.WriteLine("Failed with type: " + singleTypes[i]);

            Assert.IsTrue(success);
        }

        //header with valid type, identifier and defualt value should be good.
        [TestMethod()]
        public void ParseTestSuccess03()
        {
            int i = 0;
            bool success = true;

            for (i = singleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[vertex(" + singleTypes[i] + " " + validIdentifier + " = " + identifyApropriateValue(singleTypes[i]) + ")]");

            for (i = singleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[edge(" + singleTypes[i] + " " + validIdentifier + " = " + identifyApropriateValue(singleTypes[i]) + ")]");

            if (!success)
                Console.WriteLine("Failed with type: " + singleTypes[i]);

            Assert.IsTrue(success);
        }

        //header with valid collection type and identifier should be good.
        [TestMethod()]
        public void ParseTestSuccess04()
        {
            int i = 0, j = 0;
            bool success = true;

            for (i = collectionTypes.Count - 1; i >= 0 && success; i--)
                for (j = singleTypes.Count - 1; j >= 0 && success; j--)
                    success = TryParse("[vertex(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + ")]");

            for (i = collectionTypes.Count - 1; i >= 0 && success; i--)
                for (j = singleTypes.Count - 1; j >= 0 && success; j--)
                    success = TryParse("[edge(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + ")]");

            if (!success)
                Console.WriteLine("Failed with collection type: " + collectionTypes[i] + "<" + singleTypes[j] + ">");

            Assert.IsTrue(success);
        }

        //header with all possible types used at same time should be good.
        [TestMethod()]
        public void ParseTestSuccess5()
        {
            Assert.IsTrue(TryParse(headerWithAllTypesEdge));
            Assert.IsTrue(TryParse(headerWithAllTypesVertex));
            Assert.IsTrue(TryParse(headerWithAllTypesVertex + headerWithAllTypesEdge));
        }

        //Empty brackets should be bad.
        [TestMethod()]
        public void ParseTestFailure01()
        {
            bool success = TryParse("[]");

            Assert.IsFalse(success);
        }

        //Empty brackets with edge should be bad.
        [TestMethod()]
        public void ParseTestFailure02()
        {
            bool success = TryParse("[edge]");

            Assert.IsFalse(success);
        }

        //Empty brackets with vertex should be bad.
        [TestMethod()]
        public void ParseTestFailure03()
        {
            bool success = TryParse("[vertex]");

            Assert.IsFalse(success);
        }

        //Empty edge header should be bad.
        [TestMethod()]
        public void ParseTestFailure04()
        {
            bool success = TryParse("[edge()]");
            Assert.IsFalse(success);
        }

        //Empty vertex header should be bad.
        [TestMethod()]
        public void ParseTestFailure05()
        {
            bool success = TryParse("[vertex()]");
            Assert.IsFalse(success);
        }

        //header with valid type but invalid identifier should be bad.
        [TestMethod()]
        public void ParseTestFailure06()
        {
            int i = 0;
            bool success = false;

            for (i = singleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[vertex(" + singleTypes[i] + " " + invalidIdentifier + ")]");

            for (i = singleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[edge(" + singleTypes[i] + " " + invalidIdentifier + ")]");

            if (success)
                Console.WriteLine("It should not successfully parse with type: " + singleTypes[i] + " and invalid identifier: " + invalidIdentifier);

            Assert.IsFalse(success);
            Assert.IsFalse(success);
        }

        //header with valid type, identifier but invalid defualt value should be bad.
        [TestMethod()]
        public void ParseTestFailure07()
        {
            int i = 0;
            bool success = false;

            for (i = singleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[vertex(" + singleTypes[i] + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[i]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [vertex(" + singleTypes[i] + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[i]) + ")]");
            Assert.IsFalse(success);

            for (i = singleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[edge(" + singleTypes[i] + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[i]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [edge(" + singleTypes[i] + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[i]) + ")]");
            Assert.IsFalse(success);
        }

        //header with valid collection type and invalid identifier should be bad.
        [TestMethod()]
        public void ParseTestFailure08()
        {
            int i = 0, j = 0;
            bool success = false;

            for (i = collectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = singleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[vertex(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + invalidIdentifier + ")]");

            if (success)
                Console.WriteLine("Should not parse: [vertex(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + invalidIdentifier + ")]");
            Assert.IsFalse(success);

            for (i = collectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = singleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[edge(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + invalidIdentifier + ")]");

            if (success)
                Console.WriteLine("Should not parse: [edge(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + invalidIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //header with valid collection type, identifier and invalid defualt value should be bad.
        [TestMethod()]
        public void ParseTestFailure09()
        {
            int i = 0, j = 0;
            bool success = false;

            for (i = collectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = singleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[vertex(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[j]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [vertex(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[j]) + ")]");
            Assert.IsFalse(success);

            for (i = collectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = singleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[edge(" + collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[j]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [edge(" + collectionTypes[i] + " < " + singleTypes[j] + " > " + " " + validIdentifier + " = " + identifyInApropriateValue(singleTypes[j]) + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´"´  on a string should give error.
        [TestMethod()]
        public void ParseTestFailure10()
        {
            bool success;

            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + " = " + "\"I have forgotten to close this string" + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + validStringType + " " + validIdentifier + " = " + "\"I have forgotten to close this string" + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten ´,´ in a list should give error.
        [TestMethod()]
        public void ParseTestFailure11()
        {
            bool success;
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + "  " + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + validStringType + " " + validIdentifier + "  " + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A wrong symbol where there should be ´,´ in a list should give error.
        [TestMethod()]
        public void ParseTestFailure12()
        {
            bool success;
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + ".  " + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + validStringType + " " + validIdentifier + ".  " + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´]´ symbol should give error.
        [TestMethod()]
        public void ParseTestFailure13()
        {
            bool success;
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + ")");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + ")");
            Assert.IsFalse(success);
        }

        //A forgotten close ´)´ symbol should give error.
        [TestMethod()]
        public void ParseTestFailure14()
        {
            bool success;
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + "]");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + "]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´)]´ symbol should give error.
        [TestMethod()]
        public void ParseTestFailure15()
        {
            bool success;
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + "");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + "");
            Assert.IsFalse(success);
        }

        //A forgotten start ´(´ symbol after "vertex" or "edge" should give error.
        [TestMethod()]
        public void ParseTestFailure16()
        {
            bool success;
            success = TryParse("[vertex " + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge " + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[vertex" + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge" + validStringType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten start ´(´ symbol after "vertex" or "edge" should give error.
        [TestMethod()]
        public void ParseTestFailure17()
        {
            bool success;
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + "");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + validStringType + " " + validIdentifier + "");
            Assert.IsFalse(success);
        }
    }
}