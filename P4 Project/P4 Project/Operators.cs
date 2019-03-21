using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project
{
    class Operators
    {
        // Arithmetic and logical operators
        public const int
            OR = 1,
            AND = 2,
            EQ = 3,
            NEQ = 4,
            LESS = 5,
            GREATER = 6,
            LESSEQ = 7,
            GREATEQ = 8,
            UMIN = 9,
            PLUS = 10,
            BIMIN = 11,
            MULT = 12,
            DIV = 13,
            MOD = 14,
            NOT = 15;

        // Edge Operators
        public const int
            LEFTARR = 16,
            NONARR = 17,
            RIGHTARR = 18;

        public String getStringFromInt(int i) {
            switch (i)
            {
                case 1: return nameof(OR);
                case 2: return nameof(AND);
                case 3: return nameof(EQ);
                case 4: return nameof(NEQ);
                case 5: return nameof(LESS);
                case 6: return nameof(GREATER);
                case 7: return nameof(LESSEQ);
                case 8: return nameof(GREATEQ);
                case 9: return nameof(UMIN);
                case 10: return nameof(PLUS;
                case 11: return nameof(BIMIN);
                case 12: return nameof(MULT);
                case 13: return nameof(DIV);
                case 14: return nameof(MOD);
                case 15: return nameof(NOT);
                case 16: return nameof(LEFTARR);
                case 17: return nameof(NONARR);
                case 18: return nameof(RIGHTARR);
            }; 

        }
    }
}
