using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.Types;
using static P4_Project.TypeS;

namespace P4_Project.SymTab
{
    public class SymbolTable
    {
        Parser parser;

        public const int var = 0, func = 1; // Kinds
        public static Obj undefObj = new Obj("undef", null, var, null);

        SymbolTable parent;
        List<SymbolTable> innerScopes = new List<SymbolTable>();
        Dictionary<string, Obj> symbolDecls = new Dictionary<string, Obj>();


        //Constructor for visitor (no parser argument)
        public SymbolTable(SymbolTable _parent, Parser _parser)
        {
            parent = _parent;
            parser = _parser;
        }


        //open a new scope and make it the current (topScope)
        public SymbolTable OpenScope()
        {
            SymbolTable newTable = new SymbolTable(this, parser);
            innerScopes.Add(newTable);
            return newTable;
        }

        //close the current scope
        public SymbolTable CloseScope()
        {
            return parent;
        }


        //creates a new Object in the current scope
        public Obj NewObj(string name, BaseType type, int kind, SymbolTable scope = null)
        {
            Obj obj = new Obj(name, type, kind, scope);

            if (symbolDecls.ContainsKey(obj.Name))
                parser.SemErr($"{name} declared twice");
            else
                symbolDecls.Add(obj.Name, obj);

            return obj;
        }

        //search for a name in all open scopes and return its object node
        public Obj Find(string name)
        {
            if (symbolDecls.TryGetValue(name, out Obj value))
                return value;

            if (parent != null)
                return parent.Find(name);

            parser.SemErr($"{name} has not been declared not declared");

            return null;
        }
        // return all the innerscopes
        public List<SymbolTable> GetScopes()
        {
            return innerScopes;
        }

        // return the dictionary
        public Dictionary<string, Obj> GetDic()
        {
            return symbolDecls;
        }
    }
}
