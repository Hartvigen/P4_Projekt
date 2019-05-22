using System.Collections.Generic;

namespace P4_Project.Compiler.Interpreter.Types
{
    public class Edge
    {
        public readonly Vertex from;
        public readonly Vertex to;
        public readonly int opera;

        public readonly Dictionary<string, Value> attributes;

        public Edge(Vertex from, int opera, Vertex to, Dictionary<string, Value> attributes)
        {
            this.from = from;
            this.to = to;
            this.opera = opera;
            this.attributes = attributes;
        }
        public void UpdateAttribute(string identifier, Value value)
        {
            if (attributes.ContainsKey(identifier))
                attributes.Remove(identifier);
            attributes.Add(identifier, value);
        }

        /// <summary>
        /// Will check if given vertex pair matches this edges vertex pair.
        /// </summary>
        /// <param name="from">The vertex the edge comes from.</param>
        /// <param name="to">The vertex the edge goes to.</param>
        /// <returns>Returns true if they match the edge's from and to vertex references, false otherwise.</returns>
        public bool HasVertex(Vertex from, Vertex to) {
            return this.to == to && this.from == from;
        }

        public override string ToString()
        {
            string op;
            switch (opera)
            {
                case 16:
                    op = "<-";
                    break;
                case 17:
                    op = "--";
                    break;
                case 18:
                    op = "->";
                    break;
                default:
                    throw new System.Exception("Edge from " + from.ToString() + " to " + to.ToString() + " contains an invalid edge type");

            }
            return from.ToString() + " " + op + " " + to.ToString();
        }
    }
}
