using System;

namespace P4_Project.Types
{
    public abstract class BaseType
    {
        public abstract override string ToString();

        public static bool operator == (BaseType type1, BaseType type2)
        {
            return type1.Equals(type2); 
        }

        public static bool operator != (BaseType type1, BaseType type2)
        {
            return !type1.Equals(type2);
        }

        public override bool Equals(object obj)
        {
            return ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
