using P4_Project.AST;
using P4_Project.AST.Expressions;

namespace P4_Project.SymbolTable
{
    public class Obj
    {
        public string Name { get; }        // name of the object
        public BaseType Type { get; set; }      // type of the object
        public int Kind { get; }           // var, func ..

        public ExprNode defaultValue;  //If the symbol has a default value that must be remembered.
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
