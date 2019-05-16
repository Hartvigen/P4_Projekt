using System;
using System.Collections.Generic;

namespace P4_Project.Compiler.Interpreter
{
    internal class Scope
    {
        //Relative scopes
        private Scope _parent;
        List<Scope> _children = new List<Scope>();

        //All the variables
        private readonly Dictionary<string, Value> _values = new Dictionary<string, Value>();

        public Scope(Scope parent) {
            _parent = parent;
        }

        public void UpdateVar(string ident, Value v) {
            if (!_values.ContainsKey(ident))
                throw new Exception("Cannot update a var that doesnt exist! var: " + ident);
            _values.Remove(ident);
            _values.Add(ident, v);
        }

        public void CreateVar(string ident, Value v) {
            if (_values.ContainsKey(ident))
                throw new Exception("Cannot create var as it already exists: " + ident);
            _values.Add(ident, v);
        }

        public void TryGetValue(string ident, out Value v)
        {
            _values.TryGetValue(ident, out var v1);
            v = v1;
        }
    }
}
