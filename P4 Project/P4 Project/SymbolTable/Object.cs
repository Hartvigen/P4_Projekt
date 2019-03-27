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
        private string name;
        private Type type;
        private string hash;
        private int var;
        private int level;
        private int depth;
        private object v;

        public string GetName() { return this.name; }
        public Type GetType() { return this.type; }
        public string GetHash() { return this.hash; }
        public int GetVar() { return this.var; }
        public int GetLevel() { return this.level; }
        public int GetDepth() { return this.depth; }


        public void SetName(string name) {this.name = name; }
        public void SetType(Type type) { this.type = type ; }
        public void SetHash(string hash) { this.hash = hash; }
        public void SetVar(int var) {  this.var = var; }
        public void SetLevel(int level) {  this.level = level; }
        public void SetDepth(int depth) {  this.depth = depth; }

        public Obj(string name, Type type, string hash, int var, int level, int depth)
        {
            this.name = name;
            this.type = type;
            this.hash = hash;
            this.var = var;
            this.level = level;
            this.depth = depth;
        }

        public Obj(object hashTableObject)
        {
            this.name = hashTableObject.GetType().GetProperty("name").ToString();
            this.type = hashTableObject.GetType().GetProperty("type").GetType();
            this.hash = hashTableObject.GetType().GetProperty("hash").ToString();
            this.var = Convert.ToInt32(hashTableObject.GetType().GetProperty("var"));
            this.level = Convert.ToInt32(hashTableObject.GetType().GetProperty("level"));
            this.depth = Convert.ToInt32(hashTableObject.GetType().GetProperty("depth"));
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
            this.hashtable.Add(name, new Obj(name, type, hash, var, level, depth));
        }
    }
}
