using System;
using System.Collections.Generic;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Types;

namespace P4_Project.SymbolTable
{
    public class SymTable
    {
        private readonly Parser _parser;

        public string name;

        public const int Var = 0, Func = 1; // Kinds

        private SymTable Parent { get; set; }
        private List<SymTable> InnerScopes { get; } = new List<SymTable>();
        private readonly Dictionary<string, Obj> _symbolDecls = new Dictionary<string, Obj>();

        //Constructor for visitor (no parser argument)
        public SymTable(SymTable parent, Parser parser)
        {
            Parent = parent;
            _parser = parser;
            name = "";
        }
        
        public SymTable(SymTable parent, Parser parser, string name)
        {
            Parent = parent;
            _parser = parser;
            this.name = name;
        }


        //open a new scope and make it the current (topScope)
        public SymTable OpenScope()
        {
            SymTable newTable = new SymTable(this, _parser);
            InnerScopes.Add(newTable);
            return newTable;
        }
        
        public SymTable OpenScope(string name)
        {
            SymTable newTable = new SymTable(this, _parser, name);
            InnerScopes.Add(newTable);
            return newTable;
        }

        //close the current scope
        public SymTable CloseScope()
        {
            return Parent;
        }

        //creates a new Object in the current scope
        public Obj NewObj(string name, BaseType type, int kind, SymTable scope = null)
        {
            var obj = new Obj(name, type, kind, scope);

            if (!_symbolDecls.ContainsKey(obj.Name))
                _symbolDecls.Add(obj.Name, obj);
            else _parser.SemErr(obj.Name + " is already added to symDecls");

            return obj;
        }



        //search for a name in all open scopes and return its object node
        public Obj Find(string name)
        {
            if (_symbolDecls.TryGetValue(name, out var value))
                return value;

            if (Parent != null)
                return Parent.Find(name);

            return null;
        }

        // return all the innerScopes
        public List<SymTable> GetScopes()
        {
            return InnerScopes;
        }

        // return the dictionary
        public Dictionary<string, Obj> GetDic()
        {
            return _symbolDecls;
        }

        public void PrintAllInCurrentScope()
        {
            if (name != "")
            {
                Console.WriteLine("Named scope: " + name);
            }
            foreach (var keyValuePair in _symbolDecls)
            {
                Console.WriteLine("    Name: " + keyValuePair.Key + " Type: " + keyValuePair.Value.Type);
            }
            Parent.PrintAllInCurrentScope();
        }
        
        public void PrintAllInAllScopes()
        {
                InnerScopes.ForEach(i =>
                {
                    i.PrintAllInCurrentScope();
                });
        }
    }
}
