using System;
using System.Collections.Generic;
using System.Linq;

namespace P4_Project.Types.Functions
{
    public class FunctionType : BaseType
    {
        public BaseType ReturnType { get; set; }
        public List<BaseType> Parameters { get; private set; }


        public FunctionType(BaseType returnType, List<BaseType> parameters)
        {
            ReturnType = returnType;
            Parameters = parameters;
        }

        public override string ToString()
        {
            int i = 0;
            return "func";
           // return $"{ReturnType.ToString()} function({string.Join(", ", Parameters.Select(type => $"{type.ToString()} {{{i++}}}"))})";
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
