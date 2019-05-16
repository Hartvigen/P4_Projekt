using System.Collections.Generic;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Compiler.Interpreter.Types
{
    public class Vertex
    {
        public readonly Dictionary<string, Value> attributes = new Dictionary<string, Value>();

        public readonly string identifier;

        public readonly List<Edge> edges = new List<Edge>();

        public string name;
        public Vertex(DeclNode vertex, Dictionary<string, Value> definedAttributes)
        {
            identifier = vertex.SymbolObject.Name;

            //Insert every predefined attribute.
            foreach (var v in PreDefined.PreDefinedAttributesVertex)
                attributes.Add(v, new Value(PreDefined.GetPreDefinedValueOfPreDefinedAttributeVertex(v)));
            //override with every Defined Attribute.
            foreach (var v in definedAttributes) 
                attributes.Add(v.Key, v.Value);
        }

        public void UpdateAttribute(string ident, Value value) {
            if (attributes.ContainsKey(ident))
                attributes.Remove(ident);
            attributes.Add(ident, value);
        }
    }
}
