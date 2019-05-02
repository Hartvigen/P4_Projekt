using System.Collections.Generic;

namespace P4_Project.Types.Functions
{
    public class FunctionType : BaseType
    {
        public BaseType ReturnType { get; }
        public List<BaseType> Parameters { get; }


        public FunctionType(BaseType type, List<BaseType> parameters)
        {
            ReturnType = type;
            Parameters = parameters;
        }

        public override string ToString()
        {
            return "func";
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
