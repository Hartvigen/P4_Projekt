using System;
using System.Collections;
using System.Collections.Generic;
using P4_Project.AST;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;

namespace P4_Project.Compiler.Executor
{
    class vertex
    {
        public Dictionary<string, List<string>> Values = new Dictionary<string, List<string>>();

        private VertexDeclNode v;
        public vertex(VertexDeclNode vertex, Dictionary<string, BaseType> DefinedAttributes)
        {
            v = vertex;

            //Insert every DefineAttribute and give it value.
            foreach (var v in DefinedAttributes) {
                if (v.Key == "color")
                    Values.Add(v.Key, new List<string> { "black" });
                else Values.Add(v.Key, new List<string> { PreDefined.GetPreDefinedValueOfAttributeType(v.Value) });
            }
            overrideDefualts();
        }

        private void overrideDefualts()
        {
            foreach (var v in v.Attributes.Statements)
            {
                //Vertex decls attributes are allways assignNodes
                AssignNode a = (AssignNode)v;

                //Update the entry
                if (Values.ContainsKey(a.Target.Ident))
                    Values.Remove(a.Target.Ident);
                Values.Add(a.Target.Ident, a.Value.getValue());
            }
        }
    }
}
