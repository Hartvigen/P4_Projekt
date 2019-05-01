
using System;

namespace P4_Project.Types.Collections
{
    public abstract class CollecType : BaseType
    {
        protected BaseType SubType { get; }

        protected CollecType(BaseType subType)
        {
            SubType = subType;
        }
    }
}
