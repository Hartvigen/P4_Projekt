using System;
using System.Collections.Generic;
using P4_Project.AST;
using P4_Project.Compiler.Interpreter.Types;

namespace P4_Project.Compiler.Interpreter
{
    class Scope
    {
        //Relative scopes
        Scope parent;
        List<Scope> children = new List<Scope>();

        //All the variables
        private Dictionary<string, Value> Values = new Dictionary<string, Value>();

        public Scope(Scope parent) {
            this.parent = parent;
        }

        public void updateVar(string ident, Value v) {
            if (!Values.ContainsKey(ident))
                throw new Exception("Cannot update a var that doesnt exist! var: " + ident);
            Values.Remove(ident);
            Values.Add(ident, v);
        }

        public void createVar(string ident, Value v) {
            if (Values.ContainsKey(ident))
                throw new Exception("Cannot create var as it allready exists: " + ident);
            Values.Add(ident, v);
        }

        public bool TryGetValue(string ident, out Value v) {
            bool found = Values.TryGetValue(ident, out Value v1);                 
            v = v1;
            return found;
        }
    }
}
