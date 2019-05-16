using System.Collections.Generic;

namespace P4_Project.Compiler.Interpreter.Types
{
    public class Edge
    {
        public readonly Vertex from;
        public readonly Vertex to;
        public readonly int opera;

        public readonly Dictionary<string, Value> attributes = new Dictionary<string, Value>();

        public Edge(Vertex from, int opera, Vertex to, Dictionary<string, Value> definedAttributes)
        {
            this.from = from;
            this.to = to;
            this.opera = opera;

            //Insert every predefined attribute.
            foreach (var e in PreDefined.preDefinedAttributesEdge)
                attributes.Add(e, new Value(PreDefined.GetPreDefinedValueOfPreDefinedAttributeEdge(e)));
            //override with every Defined Attribute.
            foreach (var v in definedAttributes)
                attributes.Add(v.Key, v.Value);
        }
        public void UpdateAttribute(string identifier, Value value)
        {
            if (attributes.ContainsKey(identifier))
                attributes.Remove(identifier);
            attributes.Add(identifier, value);
        }

        public bool HasVertex(Vertex v1, Vertex v2) {
            if (v1 == from && v2 == to)
                return true;
            else if (v1 == to && v2 == from)
                return true;
            else return false;
        }
    }
}
