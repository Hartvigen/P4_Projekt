using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST;

namespace P4_Project.Compiler.Executor
{
    class vertex
    {
        public Dictionary<string, string> Values;
        public vertex(List<Tuple<string, string>> OverrideAttribueValues, Dictionary<string, BaseType> DefinedAttributes)
        {
            //Insert every DefineAttribute and give it value if possible override the defualt value.
            foreach (var v in DefinedAttributes) {
                if (v.Key == "color")
                    Values.Add(v.Key, "black");
                else Values.Add(v.Key, PreDefined.GetPreDefinedValueOfAttributeType(v.Value));
                foreach (var l in OverrideAttribueValues)
                {
                    if (l.Item1 == v.Key)
                    {
                        Values[l.Item1] = l.Item2;
                    }
                }
            }
        }
    }
}
