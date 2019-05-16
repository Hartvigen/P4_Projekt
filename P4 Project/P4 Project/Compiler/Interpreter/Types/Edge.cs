using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST.Stmts;

namespace P4_Project.Compiler.Interpreter.Types
{
    public class Edge
    {
        public Vertex from;
        public Vertex to;
        public int opera;

        public Dictionary<string, Value> attributes = new Dictionary<string, Value>();

        public Edge(Vertex from, int opera, Vertex to, Dictionary<string, Value> DefinedAttributes)
        {
            this.from = from;
            this.to = to;
            this.opera = opera;

            //Insert every predefined attribute.
            foreach (string e in PreDefined.preDefinedAttributesEdge)
                attributes.Add(e, new Value(PreDefined.GetPreDefinedValueOfPreDefinedAttributeEdge(e)));
            //override with every Defined Attribute.
            foreach (KeyValuePair<string, Value> v in DefinedAttributes)
                attributes.Add(v.Key, v.Value);
        }
        public void updateAttribute(string identifyer, Value value)
        {
            if (attributes.ContainsKey(identifyer))
                attributes.Remove(identifyer);
            attributes.Add(identifyer, value);
        }

        public bool hasVertex(Vertex v1, Vertex v2) {
            if (v1 == from && v2 == to)
                return true;
            else if (v1 == to && v2 == from)
                return true;
            else return false;
        }
    }
}
