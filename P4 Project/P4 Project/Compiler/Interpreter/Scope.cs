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
        public Dictionary<string, Value> Values = new Dictionary<string, Value>();

        public Scope(Scope parent) {
            this.parent = parent;
        }
    }
}
