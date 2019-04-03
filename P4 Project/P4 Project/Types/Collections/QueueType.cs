using System;
namespace P4_Project.Types.Collections
{
    public class QueueType : CollecType
    {
        public QueueType(BaseType subType)
        : base(subType)
        { }

        public override string ToString()
        {
            return $"queue<{SubType.ToString()}>";
        }
    }
}
