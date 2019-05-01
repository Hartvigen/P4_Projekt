using P4_Project.Types;

namespace P4_Project.SymbolTable
{
    public class Obj
    {
        public string Name { get; }        // name of the object
        public BaseType Type { get; }      // type of the object
        public int Kind { get; }           // var, func ..
        public SymTable Scope { get; }  // the scope of the object

        public Obj(string name, BaseType type, int kind, SymTable scope)
        {
            Name = name;
            Type = type;
            Kind = kind;
            Scope = scope;
        }

        public Obj()
        {
            Name = null;
            Type = null;
            Kind = 0;
            Scope = null;
        }
    }
}
