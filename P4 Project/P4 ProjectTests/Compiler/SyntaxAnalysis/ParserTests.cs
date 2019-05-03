using P4_Project.Compiler.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace P4_Project.Compiler.SyntaxAnalysis
{
    [TestFixture]
    public class ParserTests
    {
        static string validIdentifier = "test123";
        static string validTextType = "text";
        static string invalidIdentifier = "123test";

        static string invalidType = "notAType";
        static string invalidCollection = "notACollection";

        static string headerWithAllTypesVertex;
        static string headerWithAllTypesEdge;


        static List<string> singleTypes = new List<string>()
        {
            "number", "text", "boolean", "vertex", "edge"
        };

        static List<string> collectionTypes = new List<string>()
        {
            "list", "queue", "set", "stack"
        };

        //Generates a header attribute declaration for vertex and edge where all types are being used.
        [OneTimeSetUp]
        public void ClassInit()
        {
            headerWithAllTypesVertex = "[vertex( ";
            headerWithAllTypesEdge = "[edge( ";

            for (int i = collectionTypes.Count - 1; i >= 0; i--)
            {
                headerWithAllTypesVertex += singleTypes[i] + " " + validIdentifier + i + "vertex" + ", ";
                for (int j = singleTypes.Count - 1; j >= 0; j--)
                    headerWithAllTypesVertex += (collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + i + j + "vertex" + ", ");
            }

            for (int i = collectionTypes.Count - 1; i >= 0; i--)
            {
                headerWithAllTypesEdge += singleTypes[i] + " " + validIdentifier + i + "edge" + ", ";
                for (int j = singleTypes.Count - 1; j >= 0; j--)
                    headerWithAllTypesEdge += (collectionTypes[i] + "<" + singleTypes[j] + ">" + " " + validIdentifier + i + j + "edge" + ", ");
            }

            headerWithAllTypesEdge = headerWithAllTypesEdge.Remove(headerWithAllTypesEdge.LastIndexOf(","));
            headerWithAllTypesEdge += ")]";

            headerWithAllTypesVertex = headerWithAllTypesVertex.Remove(headerWithAllTypesVertex.LastIndexOf(","));
            headerWithAllTypesVertex += ")]";
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
                    case "boolean": return "true";
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
                    case "boolean": return "!";
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
        [Test]
        public void ParseTestSuccess01()
        {
            bool success = TryParse("");
            Assert.IsTrue(success);
        }

        //header with valid type and identifier should be good.
        [Test]
        public void ParseTestSuccess02()
        {
            int i = 0;
            bool success = true;

            for (i = singleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[vertex(" + singleTypes[i] + " " + validIdentifier + ")]");

            if (success)
                for (i = singleTypes.Count - 1; i >= 0 && success; i--)
                    success = TryParse("[edge(" + singleTypes[i] + " " + validIdentifier + ")]");

            if (!success)
                Console.WriteLine("Failed with type: " + singleTypes[i]);

            Assert.IsTrue(success);
        }

        //header with valid type, identifier and defualt value should be good.
        [Test]
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
        [Test]
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
        [Test]
        public void ParseTestSuccess05()
        {
            Assert.IsTrue(TryParse(headerWithAllTypesEdge));
            Assert.IsTrue(TryParse(headerWithAllTypesVertex));
            Assert.IsTrue(TryParse(headerWithAllTypesVertex + headerWithAllTypesEdge));
        }

        //tests if functions are functional with a body and use of parameters
        [Test]
        public void ParseTestSuccess6()
        {
            string func = "[vertex(boolean what = true)] func none FuncDecl(number x){vertex(v1, what = false)}";
            Assert.IsTrue(TryParse(func));

            string func2 = "[vertex(boolean tst = true)] [edge(number weight = 0)] func none FuncDecl(number x, boolean boulian){vertex{(v1, tst = false), (v2, tst = true)} v1 -> (v2, weight = 10)}";
            Assert.IsTrue(TryParse(func2));
        }

        //tests if return is accepted
        [Test]
        public void ParseTestSuccess7()
        {
            string func = "func number FuncDecl(number x){x = 5 return x}";
            Assert.IsTrue(TryParse(func));
        }

        //tests the syntax of using operators on numbers, both within and out of function declarations
        [Test]
        public void ParseTestSuccess8()
        {
            string func = "number x " +
                "x = 4 * 5 + 3 - 3";
            Assert.IsTrue(TryParse(func));

            string func2 = "func none FuncDecl(number x){x = 4 * 5 + 3 - 3}";
            Assert.IsTrue(TryParse(func2));

            string func3 = "func none FuncDecl(number x, number y){if(-x > y){x = y}}";
            Assert.IsTrue(TryParse(func3));

        }

        //testing whether declarations are correctly stored in the symbol table, by creating and then accesing a number declaration
        [Test]
        public void ParseTestSuccess9()
        {
            string numDec = "number x = 3";
            Assert.IsTrue(TryParse(numDec));

            Parser parser  = new Parser(new Scanner(StreamFromString(numDec)));
            parser.Parse();
            var x = parser.tab.Find("x");

            Assert.IsTrue(x.Name == "x");
            Assert.IsTrue(x.Kind == 0);
            Assert.IsTrue(x.Type.ToString() == "number");
        }

        //Scopes should be accessed statically, new declarations of outer scope variables should also be possible in scopes
        [Test]
        public void ParseTestSuccess10()
        {
            string scopeTest = "number x = 5 if(x == 5){number y = 4 text x = 5}";
            Assert.IsTrue(TryParse(scopeTest));

            Parser parser = new Parser(new Scanner(StreamFromString(scopeTest)));
            parser.Parse();
            var x = parser.tab.Find("x");
            var y = parser.tab.InnerScopes[0].Find("y");
            var x2 = parser.tab.InnerScopes[0].Find("x");

            Assert.IsTrue(x.Name == "x");
            Assert.IsTrue(x.Kind == 0);
            Assert.IsTrue(x.Type.ToString() == "number");

            Assert.IsTrue(y.Name == "y");
            Assert.IsTrue(y.Kind == 0);
            Assert.IsTrue(y.Type.ToString() == "number");

            Assert.IsTrue(x2.Name == "x");
            Assert.IsTrue(x2.Kind == 0);
            Assert.IsTrue(x2.Type.ToString() == "text");
        }

        //Empty brackets should be bad.
        [Test]
        public void ParseTestFailure01()
        {
            bool success = TryParse("[]");

            Assert.IsFalse(success);
        }

        //Empty brackets with edge should be bad.
        [Test]
        public void ParseTestFailure02()
        {
            bool success = TryParse("[edge]");

            Assert.IsFalse(success);
        }

        //Empty brackets with vertex should be bad.
        [Test]
        public void ParseTestFailure03()
        {
            bool success = TryParse("[vertex]");

            Assert.IsFalse(success);
        }

        //Empty edge header should be bad.
        [Test]
        public void ParseTestFailure04()
        {
            bool success = TryParse("[edge()]");
            Assert.IsFalse(success);
        }

        //Empty vertex header should be bad.
        [Test]
        public void ParseTestFailure05()
        {
            bool success = TryParse("[vertex()]");
            Assert.IsFalse(success);
        }

        //header with valid type but invalid identifier should be bad.
        [Test]
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
        [Test]
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
        [Test]
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
        [Test]
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
        [Test]
        public void ParseTestFailure10()
        {
            bool success;

            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + " = " + "\"I have forgotten to close this string" + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + validTextType + " " + validIdentifier + " = " + "\"I have forgotten to close this string" + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten ´,´ in a list should give error.
        [Test]
        public void ParseTestFailure11()
        {
            bool success;
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + "  " + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + validTextType + " " + validIdentifier + "  " + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A wrong symbol where there should be ´,´ in a list should give error.
        [Test]
        public void ParseTestFailure12()
        {
            bool success;
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + ".  " + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + validTextType + " " + validIdentifier + ".  " + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´]´ symbol should give error.
        [Test]
        public void ParseTestFailure13()
        {
            bool success;
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + ")");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + ")");
            Assert.IsFalse(success);
        }

        //A forgotten close ´)´ symbol should give error.
        [Test]
        public void ParseTestFailure14()
        {
            bool success;
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + "]");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + "]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´)]´ symbol should give error.
        [Test]
        public void ParseTestFailure15()
        {
            bool success;
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + "");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + "");
            Assert.IsFalse(success);
        }

        //A forgotten start ´(´ symbol after "vertex" or "edge" should give error.
        [Test]
        public void ParseTestFailure16()
        {
            bool success;
            success = TryParse("[vertex " + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge " + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[vertex" + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge" + validTextType + " " + validIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten end ´(´ symbol after "vertex" or "edge" should give error.
        [Test]
        public void ParseTestFailure17()
        {
            bool success;
            success = TryParse("[vertex(" + validTextType + " " + validIdentifier + "");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + validTextType + " " + validIdentifier + "");
            Assert.IsFalse(success);
        }

        // a forgotten func before FuncDecl should give error
        [Test]
        public void ParseTestFailure18()
        {
            string func = "FuncDecl(number x){x = 5 return x}";
            Assert.IsFalse(TryParse(func));
        }

        // a forgotten FuncDecl before function should give error
        [Test]
        public void ParseTestFailure19()
        {
            string func = "func(number x){x = 5}";
            Assert.IsFalse(TryParse(func));
        }

        // a forgotten FuncDecl before func should give error
        [Test]
        public void ParseTestFailure20()
        {
            string str = "[vertex(edge " + validIdentifier + ")]";
            Assert.IsTrue(TryParse(str));
        }

        //Invalid single type is error
        [Test]
        public void ParseTestFailure21()
        {
            string str = "[vertex(" + invalidType + " " + validIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid single type and invalid identfier is error
        [Test]
        public void ParseTestFailure22()
        {
            string str = "[vertex(" + invalidType + " " + invalidIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid collection with valid single type and valid identifier is error
        [Test]
        public void ParseTestFailure23()
        {
            string str = "[vertex(" + invalidCollection + "<" + singleTypes[0] + ">" + " " + validIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid collection with invalid single type and valid identifier is error
        [Test]
        public void ParseTestFailure24()
        {
            string str = "[vertex(" + invalidCollection + "<" + invalidType + ">" + " " + validIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid collection with invalid single type and invalid identifier is error
        [Test]
        public void ParseTestFailure25()
        {
            string str = "[vertex(" + invalidCollection + "<" + invalidType + ">" + " " + invalidIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }
    }
}