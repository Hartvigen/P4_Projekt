using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace P4_Project.SymTab
{
    public class Obj
    {
        public string Name { get; set; }    // name of the object
        public int Type { get; set; }       // type of the object (undef for funcs)
        public int Kind { get; set; }       // var, func


        //Consturctor for undefObj in synbolTable
        public Obj(string name, int type, int kind)
        {
            Name = name;
            Type = type;
            Kind = kind;
        }

    }
}
