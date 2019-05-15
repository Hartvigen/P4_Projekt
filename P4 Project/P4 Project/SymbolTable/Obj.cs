using P4_Project.AST;

namespace P4_Project.SymbolTable
{
    public class Obj
    {
        public string Name { get; }        // name of the object
        public BaseType Type { get; }      // type of the object
        public int Kind { get; }           // var, func ..

        public Obj(string name, BaseType type, int kind)
        {
            Name = name;
            Type = type;
            Kind = kind;
        }

        public Obj()
        {
            Name = null;
            Type = null;
            Kind = 0;
        }
    }
}
