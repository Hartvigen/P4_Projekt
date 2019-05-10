using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.AST;

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

        public static string GetPreDefinedValueOfAttributeType(BaseType type)
        {
            if (type.name == "number")
                return "0";
            if (type.name == "text")
                return "";
            if (type.name == "vertex" || type.name == "edge" || type.name == "collec")
                return "none";
            throw new Exception("Type has no predefined value! :: " + type.name + " ::");
        }

        public static string getTypeOfAttribute(string name)
        {
            return "text";
        }
    }
}
