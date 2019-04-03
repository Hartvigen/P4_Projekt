using System;
namespace P4_Project.Types.Collections
{
    public class StackType : CollecType
    {
        public StackType(BaseType subType) 
        : base(subType)
        { }

        public override string ToString()
        {
            return $"stack<{SubType.ToString()}>";
        }
    }
}
