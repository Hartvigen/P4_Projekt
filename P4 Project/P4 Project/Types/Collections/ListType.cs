using System;
namespace P4_Project.Types.Collections
{
    public class ListType : CollecType
    {
        public ListType(BaseType subType)
        : base(subType)
        { }

        public override string ToString()
        {
            return $"list<{SubType.ToString()}>";
        }
    }
}
