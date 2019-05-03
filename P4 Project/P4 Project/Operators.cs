using System;
using System.Collections.Generic;
using P4_Project.AST;

namespace P4_Project
{
    internal static class Operators
    {
        // Arithmetic and logical operators
        public const int
            Or = 1,
            And = 2,
            Eq = 3,
            Neq = 4,
            Less = 5,
            Greater = 6,
            Lesseq = 7,
            Greateq = 8,
            Umin = 9,
            Plus = 10,
            Bimin = 11,
            Mult = 12,
            Div = 13,
            Mod = 14,
            Not = 15;

        // Edge Operators
        public const int
            Leftarr = 16,
            Nonarr = 17,
            Rightarr = 18;

        public static string GetNameFromInt(int i)
        {
            switch (i)
            {
                case 1: return nameof(Or);
                case 2: return nameof(And);
                case 3: return nameof(Eq);
                case 4: return nameof(Neq);
                case 5: return nameof(Less);
                case 6: return nameof(Greater);
                case 7: return nameof(Lesseq);
                case 8: return nameof(Greateq);
                case 9: return nameof(Umin);
                case 10: return nameof(Plus);
                case 11: return nameof(Bimin);
                case 12: return nameof(Mult);
                case 13: return nameof(Div);
                case 14: return nameof(Mod);
                case 15: return nameof(Not);
                case 16: return nameof(Leftarr);
                case 17: return nameof(Nonarr);
                case 18: return nameof(Rightarr);
                default: throw new Exception("Operator type: " + i + " does not have a Name associated");
            }
        }

        public static string GetCodeFromInt(int i)
        {
            //&lt; is replaced in xml as < is a keyword in XML.
            //&gt; is replaced in xml as < is a keyword in XML.
            //&amp; is replaced in xml as & is a keyword in XML.
            switch (i)
            {
                case 1: return "||";
                case 2: return "&&";
                case 3: return "==";
                case 4: return "!=";
                case 5: return "<";
                case 6: return ">";
                case 7: return "<=";
                case 8: return ">=";
                case 9: return "-";
                case 10: return "+";
                case 11: return "-";
                case 12: return "*";
                case 13: return "/";
                case 14: return "%";
                case 15: return "!";
                case 16: return "<-";
                case 17: return "--";
                case 18: return "->";
                default: throw new Exception("Operator type: " + i + " does not have a Code associated");
            }
        }

        public static List<BaseType> GetOperandTypeFromInt(int i)
        {
            switch (i)
            {
                case 1:
                case 2:
                case 15: return new List<BaseType> {new BaseType("boolean")};

                case 3:
                case 4:
                    return new List<BaseType>
                    {
                        new BaseType("number"), new BaseType("text"), new BaseType("boolean"), new BaseType("vertex"), new BaseType("edge")
                    };

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    return new List<BaseType> {new BaseType("number")};

                case 16:
                case 17:
                case 18:
                    return new List<BaseType> {new BaseType("vertex")};

                default: throw new Exception("Operator type: " + i + " does not have a Code associated");
            }
        }

        public static BaseType GetResultingTypeFromOperandTypeAndOperator(BaseType type, int i)
        {
            switch (i)
            {
                case 10:
                    if (type.name == "text")
                        return new BaseType("text");
                    else if(type.name == "number")
                        return new BaseType("number");
                    break;
                case 9: 
                case 11: 
                case 12: 
                case 13: 
                case 14:
                    if (type.name == "number")
                        return new BaseType("number");
                    break;
                case 3: 
                case 4: 
                        return new BaseType("boolean");
                case 5: 
                case 6: 
                case 7: 
                case 8:
                    if (type.name == "number")
                        return new BaseType("boolean");
                    break;
                case 1: 
                case 2: 
                case 15:
                    if (type.name == "boolean")
                        return new BaseType("boolean");
                    break;
                case 16: 
                case 17: 
                case 18:
                    if (type.name == "vertex")
                        return new BaseType("none");
                    break;
               default: return null;
            }
            return null;
        }
    }
}