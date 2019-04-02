using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace P4_Project.SymbolTable
{
    public class Obj
    {
        public string Name { get; set; }    // name of the object
        public int Type { get; set; }       // type of the object (undef for funcs)
        public Obj Next { get; set; }       // to next object in same scope
        public int Kind { get; set; }       // var, func, scope
        public int Adr { get; set; }        // address in memory or start of func
        public int Level { get; set; }      // nesting level; 0=global, 1=local
        public Obj Locals { get; set; }     // scopes: to locally declared objects
        public int NextAdr { get; set; }    // scopes: next free address in this scope 

        //Empty constructor for newObj in SymbolTable
        public Obj() { }

        //Constructor for OpenScope function
        public Obj(string name, int kind, Obj locals, Obj next, int nextAdr)
        {
            Name = name;
            Next = next;
            Kind = kind;
            Locals = locals;
            NextAdr = nextAdr;
        }

        //Consturctor for undefObj in synbolTable
        public Obj(string name, int kind, Obj next, int level, int type, int adr)
        {
            Name = name;
            Kind = kind;
            Next = next;
            Level = level;
            Type = type;
            Adr = adr;
        }

    }
}
