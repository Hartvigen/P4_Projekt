using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using P4_Project.Types;

namespace P4_Project.SymTab
{
    public class Obj
    {
        public string Name { get; set; }        // name of the object
        public BaseType Type { get; set; }      // type of the object
        public int Kind { get; set; }           // var, func ..

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
