using P4_Project.AST;

namespace P4_Project.SymbolTable
{
    public class Obj
    {
        public string Name { get; }        // name of the object
        public BaseType type { get; }      // type of the object
        public int Kind { get; }           // var, func ..
        public SymTable Scope { get; }  // the scope of the object

        public Obj(string name, BaseType type, int kind, SymTable scope)
        {
            Name = name;
            this.type = type;
            Kind = kind;
        }

        public Obj()
        {
            Name = null;
            type = null;
            Kind = 0;
        }
    }
}
