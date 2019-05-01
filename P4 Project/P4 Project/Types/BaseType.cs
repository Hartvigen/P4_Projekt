namespace P4_Project.Types
{
    public abstract class BaseType
    {
        public abstract override string ToString();

        public static bool operator == (BaseType type1, BaseType type2)
        {
            if (type1 is null || type2 is null)
                return false;
            return type1.Equals(type2); 
        }

        public static bool operator != (BaseType type1, BaseType type2)
        {
            if (type1 is null || type2 is null)
                return false;
            return !type1.Equals(type2);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
