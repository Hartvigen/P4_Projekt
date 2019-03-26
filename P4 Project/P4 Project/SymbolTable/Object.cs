using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace P4_Project.SymbolTable
{
    public class Object
    {
         public String name;
         public Type type;
         public String hash;
         public int var;
         public int level;
         public int depth;

        public Object(string name, Type type, string hash, int var, int level, int depth)
        {
            this.name = name;
            this.type = type;
            this.hash = hash;
            this.var = var;
            this.level = level;
            this.depth = depth;
        }
    }
    public class SymbolTableClass
    {
        public Hashtable hashtable;

        public SymbolTableClass()
        {
        }

        public SymbolTableClass(string name, Type type, string hash, int var, int level, int depth)
        {
            this.hashtable.Add(name, new Object(name, type, hash, var, level, depth));
        }
    }
}
