using System;
namespace P4_Project.Types.Collections
{
    public abstract class CollecType : BaseType
    {
        public BaseType SubType { get; private set; }

        protected CollecType(BaseType subType)
        {
            SubType = subType;
        }
    }
}
