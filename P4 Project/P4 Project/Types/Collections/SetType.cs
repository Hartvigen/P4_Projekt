
namespace P4_Project.Types.Collections
{
    public class SetType : CollecType
    {
        public SetType(BaseType subType)
        : base(subType)
        { }

        public override string ToString()
        {
            return $"set<{SubType}>";
        }
    }
}
