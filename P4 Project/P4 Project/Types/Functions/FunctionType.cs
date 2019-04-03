using System;
using System.Collections.Generic;
using System.Linq;

namespace P4_Project.Types.Functions
{
    public class FunctionType : BaseType
    {
        public Dictionary<string, BaseType> Parameters { get; private set; }
        public List<BaseType> TypeList { get => Parameters.Values.ToList(); }

        public FunctionType(Dictionary<string, BaseType> parameters)
        {
            Parameters = parameters;
        }

        public override string ToString()
        {
            return $"function({string.Join(", ", Parameters.Select(pair => $"{pair.Key} {{{pair.Value.ToString()}}}"))})";
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
