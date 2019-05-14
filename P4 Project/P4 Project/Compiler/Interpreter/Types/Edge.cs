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

        public Dictionary<string, List<Value>> attributes = new Dictionary<string, List<Value>>();

        public EdgeCreateNode edge;

        public Edge(Vertex from, Vertex to, int opera, EdgeCreateNode edge, Dictionary<string, List<Value>> DefinedAttributes)
        {
            this.from = from;
            this.to = to;
            this.opera = opera;
            this.edge = edge;

            //Insert every DefineAttribute and give it value.
            foreach (KeyValuePair<string, List<Value>> v in DefinedAttributes)
            {
                attributes.Add(v.Key, v.Value);
            }
        }
        public void updateAttribute(string identifyer, List<Value> value)
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
