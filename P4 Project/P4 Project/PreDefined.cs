using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project
{
    public abstract class PreDefined
    {
        public static List<string> preDefinedFunctions = new List<string>
        {
                 "GetEdge",
                "RemoveEdge",
                 "GetEdges",
              "RemoveVertex",
                 "GetVertices",
                "ClearEdges",
                "ClearVertices",
               "ClearAll"
        };

        public static List<string> preDefinedAttributesVertex = new List<string>
        {
            "label",
            "color"
        };

        public static List<string> preDefinedAttributesEdge = new List<string>
        {
            "label",
            "color",
            "style"
        };
        public static string getTypeOfAttribute(string name)
        {
            return "text";
        }
    }
}
