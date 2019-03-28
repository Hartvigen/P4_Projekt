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
        private string _name;
        private Type _type;
        private string _hash;
        private Obj _var;
        private int _level;
        private int _depth;

        public string Name { get => _name; set => _name = value; }
        public Type Type { get => _type; set => _type = value; }
        public string Hash { get => _hash; set => _hash = value; }
        public Obj Var { get => _var; set => _var = value; }
        public int Level { get => _level; set => _level = value; }
        public int Depth { get => _depth; set => _depth = value; }

        public Obj(string name, Type type, string hash, Obj var, int level, int depth)
        {
            this._name = name;
            this._type = type;
            this._hash = hash;
            this._var = var;
            this._level = level;
            this._depth = depth;
        }

        public Obj(object hashTableObject)
        {
            this._name = hashTableObject.GetType().GetProperty("name").ToString();
            this._type = hashTableObject.GetType().GetProperty("type").GetType();
            this._hash = hashTableObject.GetType().GetProperty("hash").ToString();
            this._var = null;
            this._level = Convert.ToInt32(hashTableObject.GetType().GetProperty("level"));
            this._depth = Convert.ToInt32(hashTableObject.GetType().GetProperty("depth"));
        }
    }
    public class SymbolTableClass
    {
        public Hashtable hashtable;

        public SymbolTableClass()
        {
        }

        public SymbolTableClass(string name, Type type, string hash, Obj var, int level, int depth)
        {
            this.hashtable.Add(name, new Obj(name, type, hash, var, level, depth));
        }
    }
}
