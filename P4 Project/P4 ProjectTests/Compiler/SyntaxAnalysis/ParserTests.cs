using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using P4_Project.Compiler.SyntaxAnalysis;

namespace P4_ProjectTests1.Compiler.SyntaxAnalysis
{
    [TestFixture]
    public sealed class ParserTests
    {
        private string ValidIdentifier { get; } = "test123";
        private string ValidTextType { get; } = "text";
        private string InvalidIdentifier { get; } = "123test";

        private string InvalidType { get; } = "notAType";
        private string InvalidCollection { get; } = "notACollection";

        private static string _headerWithAllTypesVertex;
        private static string _headerWithAllTypesEdge;


        private static readonly List<string> SingleTypes = new List<string>()
        {
            "number", "text", "boolean", "vertex", "edge"
        };

        private static readonly List<string> CollectionTypes = new List<string>()
        {
            "list", "queue", "set", "stack"
        };

        //Generates a header attribute declaration for vertex and edge where all types are being used.
        [OneTimeSetUp]
        public void ClassInit()
        {
            _headerWithAllTypesVertex = "[vertex( ";
            _headerWithAllTypesEdge = "[edge( ";

            for (int i = CollectionTypes.Count - 1; i >= 0; i--)
            {
                _headerWithAllTypesVertex += SingleTypes[i] + " " + ValidIdentifier + i + "vertex" + ", ";
                for (int j = SingleTypes.Count - 1; j >= 0; j--)
                    _headerWithAllTypesVertex += (CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + ValidIdentifier + i + j + "vertex" + ", ");
            }

            for (int i = CollectionTypes.Count - 1; i >= 0; i--)
            {
                _headerWithAllTypesEdge += SingleTypes[i] + " " + ValidIdentifier + i + "edge" + ", ";
                for (int j = SingleTypes.Count - 1; j >= 0; j--)
                    _headerWithAllTypesEdge += (CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + ValidIdentifier + i + j + "edge" + ", ");
            }

            _headerWithAllTypesEdge = _headerWithAllTypesEdge.Remove(_headerWithAllTypesEdge.LastIndexOf(",", StringComparison.Ordinal));
            _headerWithAllTypesEdge += ")]";

            _headerWithAllTypesVertex = _headerWithAllTypesVertex.Remove(_headerWithAllTypesVertex.LastIndexOf(",", StringComparison.Ordinal));
            _headerWithAllTypesVertex += ")]";
        }

        //Will identify an Appropriate Value given a type.
        private static string IdentifyAppropriateValue(string type)
        {
            if (CollectionTypes.Contains(type))
                switch (type)
                {
                    case "list": return "";
                    case "set": return "";
                    case "stack": return "";
                    case "queue": return "";
                    default: throw new Exception(type + " is not added to the switch but is in the list.");
                }
            else if (SingleTypes.Contains(type))
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

        //Will identify an in appropriate value given a type.
        private static string IdentifyInAppropriateValue(string type)
        {
            if (CollectionTypes.Contains(type))
                switch (type)
                {
                    case "list": return "!";
                    case "set": return "!";
                    case "stack": return "!";
                    case "queue": return "!";
                    default: throw new Exception(type + " is not added to the switch but is in the list of collections.");
                }
            else if (SingleTypes.Contains(type))
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
        private static bool TryParse(string program)
        {
            var parser
                = new Parser(
                    new Scanner(
                        StreamFromString(program)
                ));

            parser.Parse();

            return parser.errors.count == 0;
        }

        private static MemoryStream StreamFromString(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        //The Empty String Should be good.  
        [Test]
        public void ParseTestSuccess01()
        {
            var success = TryParse("");
            Assert.IsTrue(success);
        }

        //header with valid type and identifier should be good.
        [Test]
        public void ParseTestSuccess02()
        {
            var success = true;
            int i;

            for (i = SingleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[vertex(" + SingleTypes[i] + " " + ValidIdentifier + ")]");

            if (success)
                for (i = SingleTypes.Count - 1; i >= 0 && success; i--)
                    success = TryParse("[edge(" + SingleTypes[i] + " " + ValidIdentifier + ")]");

            if (!success)
                Console.WriteLine("Failed with type: " + SingleTypes[i]);

            Assert.IsTrue(success);
        }

        //header with valid type, identifier and default value should be good.
        [Test]
        public void ParseTestSuccess03()
        {
            int i;
            var success = true;

            for (i = SingleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[vertex(" + SingleTypes[i] + " " + ValidIdentifier + " = " + IdentifyAppropriateValue(SingleTypes[i]) + ")]");

            for (i = SingleTypes.Count - 1; i >= 0 && success; i--)
                success = TryParse("[edge(" + SingleTypes[i] + " " + ValidIdentifier + " = " + IdentifyAppropriateValue(SingleTypes[i]) + ")]");

            if (!success)
                Console.WriteLine("Failed with type: " + SingleTypes[i]);

            Assert.IsTrue(success);
        }

        //header with valid collection type and identifier should be good.
        [Test]
        public void ParseTestSuccess04()
        {
            int i, j = 0;
            var success = true;

            for (i = CollectionTypes.Count - 1; i >= 0 && success; i--)
                for (j = SingleTypes.Count - 1; j >= 0 && success; j--)
                    success = TryParse("[vertex(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + ValidIdentifier + ")]");

            for (i = CollectionTypes.Count - 1; i >= 0 && success; i--)
                for (j = SingleTypes.Count - 1; j >= 0 && success; j--)
                    success = TryParse("[edge(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + ValidIdentifier + ")]");

            if (!success)
                Console.WriteLine("Failed with collection type: " + CollectionTypes[i] + "<" + SingleTypes[j] + ">");

            Assert.IsTrue(success);
        }

        //header with all possible types used at same time should be good.
        [Test]
        public void ParseTestSuccess05()
        {
            Assert.IsTrue(TryParse(_headerWithAllTypesEdge));
            Assert.IsTrue(TryParse(_headerWithAllTypesVertex));
            Assert.IsTrue(TryParse(_headerWithAllTypesVertex + _headerWithAllTypesEdge));
        }

        //tests if functions are functional with a body and use of parameters
        [Test]
        public void ParseTestSuccess6()
        {
            const string func = "[vertex(boolean what = true)] func none FuncDecl(number x){vertex(v1, what = false)}";
            Assert.IsTrue(TryParse(func));

            const string func2 = "[vertex(boolean tst = true)] [edge(number weight = 0)] func none FuncDecl(number x, boolean belgian){vertex{(v1, tst = false), (v2, tst = true)} v1 -> (v2, weight = 10)}";
            Assert.IsTrue(TryParse(func2));
        }

        //tests if return is accepted
        [Test]
        public void ParseTestSuccess7()
        {
            const string func = "func number FuncDecl(number x){x = 5 return x}";
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
            var success = TryParse("[edge()]");
            Assert.IsFalse(success);
        }

        //Empty vertex header should be bad.
        [Test]
        public void ParseTestFailure05()
        {
            var success = TryParse("[vertex()]");
            Assert.IsFalse(success);
        }

        //header with valid type but invalid identifier should be bad.
        [Test]
        public void ParseTestFailure06()
        {
            int i;
            var success = false;

            for (i = SingleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[vertex(" + SingleTypes[i] + " " + InvalidIdentifier + ")]");

            for (i = SingleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[edge(" + SingleTypes[i] + " " + InvalidIdentifier + ")]");

            if (success)
                Console.WriteLine("It should not successfully parse with type: " + SingleTypes[i] + " and invalid identifier: " + InvalidIdentifier);

            Assert.IsFalse(success);
            Assert.IsFalse(success);
        }

        //header with valid type, identifier but invalid default value should be bad.
        [Test]
        public void ParseTestFailure07()
        {
            int i;
            var success = false;

            for (i = SingleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[vertex(" + SingleTypes[i] + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[i]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [vertex(" + SingleTypes[i] + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[i]) + ")]");
            Assert.IsFalse(success);

            for (i = SingleTypes.Count - 1; i >= 0 && !success; i--)
                success = TryParse("[edge(" + SingleTypes[i] + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[i]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [edge(" + SingleTypes[i] + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[i]) + ")]");
            Assert.IsFalse(success);
        }

        //header with valid collection type and invalid identifier should be bad.
        [Test]
        public void ParseTestFailure08()
        {
            int i, j = 0;
            var success = false;

            for (i = CollectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = SingleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[vertex(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + InvalidIdentifier + ")]");

            if (success)
                Console.WriteLine("Should not parse: [vertex(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + InvalidIdentifier + ")]");
            Assert.IsFalse(success);

            for (i = CollectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = SingleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[edge(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + InvalidIdentifier + ")]");

            if (success)
                Console.WriteLine("Should not parse: [edge(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + InvalidIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //header with valid collection type, identifier and invalid default value should be bad.
        [Test]
        public void ParseTestFailure09()
        {
            int i, j = 0;
            var success = false;

            for (i = CollectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = SingleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[vertex(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[j]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [vertex(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[j]) + ")]");
            Assert.IsFalse(success);

            for (i = CollectionTypes.Count - 1; i >= 0 && !success; i--)
                for (j = SingleTypes.Count - 1; j >= 0 && !success; j--)
                    success = TryParse("[edge(" + CollectionTypes[i] + "<" + SingleTypes[j] + ">" + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[j]) + ")]");

            if (success)
                Console.WriteLine("Should not parse: [edge(" + CollectionTypes[i] + " < " + SingleTypes[j] + " > " + " " + ValidIdentifier + " = " + IdentifyInAppropriateValue(SingleTypes[j]) + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´"´  on a string should give error.
        [Test]
        public void ParseTestFailure10()
        {
            var success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + " = " + "\"I have forgotten to close this string" + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + ValidTextType + " " + ValidIdentifier + " = " + "\"I have forgotten to close this string" + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten ´,´ in a list should give error.
        [Test]
        public void ParseTestFailure11()
        {
            var success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + "  " + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + ValidTextType + " " + ValidIdentifier + "  " + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A wrong symbol where there should be ´,´ in a list should give error.
        [Test]
        public void ParseTestFailure12()
        {
            var success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + ".  " + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + ValidTextType + " " + ValidIdentifier + ".  " + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´]´ symbol should give error.
        [Test]
        public void ParseTestFailure13()
        {
            var success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + ")");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + ")");
            Assert.IsFalse(success);
        }

        //A forgotten close ´)´ symbol should give error.
        [Test]
        public void ParseTestFailure14()
        {
            var success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + "]");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + "]");
            Assert.IsFalse(success);
        }

        //A forgotten close ´)]´ symbol should give error.
        [Test]
        public void ParseTestFailure15()
        {
            var success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + "");
            Assert.IsFalse(success);
            success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + "");
            Assert.IsFalse(success);
        }

        //A forgotten start ´(´ symbol after "vertex" or "edge" should give error.
        [Test]
        public void ParseTestFailure16()
        {
            var success = TryParse("[vertex " + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge " + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[vertex" + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
            success = TryParse("[edge" + ValidTextType + " " + ValidIdentifier + ")]");
            Assert.IsFalse(success);
        }

        //A forgotten end ´(´ symbol after "vertex" or "edge" should give error.
        [Test]
        public void ParseTestFailure17()
        {
            var success = TryParse("[vertex(" + ValidTextType + " " + ValidIdentifier + "");
            Assert.IsFalse(success);
            success = TryParse("[edge(" + ValidTextType + " " + ValidIdentifier + "");
            Assert.IsFalse(success);
        }

        // a forgotten func before FuncDecl should give error
        [Test]
        public void ParseTestFailure18()
        {
            const string func = "FuncDecl(number x){x = 5 return x}";
            Assert.IsFalse(TryParse(func));
        }

        // a forgotten FuncDecl before function should give error
        [Test]
        public void ParseTestFailure19()
        {
            const string func = "func(number x){x = 5}";
            Assert.IsFalse(TryParse(func));
        }

        // a forgotten FuncDecl before func should give error
        [Test]
        public void ParseTestFailure20()
        {
            var str = "[vertex(edge " + ValidIdentifier + ")]";
            Assert.IsTrue(TryParse(str));
        }

        //Invalid single type is error
        [Test]
        public void ParseTestFailure21()
        {
            var str = "[vertex(" + InvalidType + " " + ValidIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid single type and invalid identifier is error
        [Test]
        public void ParseTestFailure22()
        {
            var str = "[vertex(" + InvalidType + " " + InvalidIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid collection with valid single type and valid identifier is error
        [Test]
        public void ParseTestFailure23()
        {
            var str = "[vertex(" + InvalidCollection + "<" + SingleTypes[0] + ">" + " " + ValidIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid collection with invalid single type and valid identifier is error
        [Test]
        public void ParseTestFailure24()
        {
            var str = "[vertex(" + InvalidCollection + "<" + InvalidType + ">" + " " + ValidIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }

        //Invalid collection with invalid single type and invalid identifier is error
        [Test]
        public void ParseTestFailure25()
        {
            var str = "[vertex(" + InvalidCollection + "<" + InvalidType + ">" + " " + InvalidIdentifier + ")]";
            Assert.IsFalse(TryParse(str));
        }
    }
}