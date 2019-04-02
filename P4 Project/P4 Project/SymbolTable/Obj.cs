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
        private string _name; // name of the object
        private int _type;    // type of the object (undef for procs)
        private Obj _next;    // to next object in same scope
        private int _kind;    // var, proc, scope
        private int _adr;     // address in memory or start of proc
        private int _level;   // nesting level; 0=global, 1=local
        private Obj _locals;  // scopes: to locally declared objects
        private int _nextAdr; // scopes: next free address in this scope 

        public string Name { get => _name; set => _name = value; }
        public int Type { get => _type; set => _type = value; }
        public Obj Next { get => _next; set => _next = value; }
        public int Kind { get => _kind; set => _kind = value; }
        public int Adr { get => _adr; set => _adr = value; }
        public int Level { get => _level; set => _level = value; }
        public Obj Locals { get => _locals; set => _locals = value; }
        public int NextAdr { get => _nextAdr; set => _nextAdr = value; }

        //Empty constructor for newObj in SymbolTable
        public Obj() { }

        //Constructor for OpenScope function
        public Obj(string name, int kind, Obj locals, Obj next, int nextAdr)
        {
            _name = name;
            _next = next;
            _kind = kind;
            _locals = locals;
            _nextAdr = nextAdr;
        }

        //Consturctor for undefObj in synbolTable
        public Obj(string name, int kind, Obj next, int level, int type, int adr)
        {
            _name = name;
            _kind = kind;
            _next = next;
            _level = level;
            _type = type;
            _adr = adr;
        }

    }
}
