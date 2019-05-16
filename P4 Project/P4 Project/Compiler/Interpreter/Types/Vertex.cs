using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Compiler.Interpreter.Types
{
    public class Vertex
    {
        public Dictionary<string, Value> attributes = new Dictionary<string, Value>();

        public string identifyer;

        public List<Edge> edge = new List<Edge>();

        private VertexDeclNode v;

        public string name;
        public Vertex(VertexDeclNode vertex, Dictionary<string, Value> DefinedAttributes)
        {
            v = vertex;

            identifyer = v.SymbolObject.Name;

            //Insert every predefined attribute.
            foreach (string v in PreDefined.preDefinedAttributesVertex)
                attributes.Add(v, new Value(PreDefined.GetPreDefinedValueOfPreDefinedAttributeVertex(v)));
            //override with every Defined Attribute.
            foreach (KeyValuePair<string, Value> v in DefinedAttributes) 
                attributes.Add(v.Key, v.Value);
        }

        public void updateAttribute(string identifyer, Value value) {
            if (attributes.ContainsKey(identifyer))
                attributes.Remove(identifyer);
            attributes.Add(identifyer, value);
        }
    }
}
