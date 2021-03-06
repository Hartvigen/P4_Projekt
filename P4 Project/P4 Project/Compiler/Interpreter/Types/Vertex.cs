﻿using System.Collections.Generic;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Compiler.Interpreter.Types
{
    public class Vertex
    {
        public readonly Dictionary<string, Value> attributes;

        public readonly string identifier;

        public readonly List<Edge> edges = new List<Edge>();

        public string name;
        public Vertex(DeclNode vertex, Dictionary<string, Value> attributes)
        {
            identifier = vertex.SymbolObject.Name;
            this.attributes = attributes;
        }

        public void UpdateAttribute(string ident, Value value) {
            if (attributes.ContainsKey(ident))
                attributes.Remove(ident);
            attributes.Add(ident, value);
        }

        public override string ToString()
        {
            return identifier;
        }
    }
}
