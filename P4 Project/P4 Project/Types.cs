using System;

namespace P4_Project
{
    public static class Types
    {
        // Types
        public const int
            undef = 0,
            number = 1,
            boolean = 2,
            text = 3,
            vertex = 4,
            edge = 5,
            set = 10,
            list = 20,
            queue = 30,
            stack = 40;

        // Object kinds
        public const int
            var = 0,
            proc = 1;


        public static string getCodeFromInt(int i)
        {
            string s = i.ToString();

            if (i < 10)
                return getStringFromInt(i);

            string collectiontype;
            collectiontype = getStringFromInt((i / 10) * 10);
            collectiontype += "<" + getStringFromInt(i % 10) + ">";
            return collectiontype;
        }

        private static string getStringFromInt(int i)
        {
            switch (i)
            {
                case 0: return nameof(undef);
                case 1: return nameof(number);
                case 2: return nameof(boolean);
                case 3: return nameof(text);
                case 4: return nameof(vertex);
                case 5: return nameof(edge);
                case 10: return nameof(set);
                case 20: return nameof(list);
                case 30: return nameof(queue);
                case 40: return nameof(stack);
                default: throw new Exception("Not vaild type");
            }
        }
    }
}
