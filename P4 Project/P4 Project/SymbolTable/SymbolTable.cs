using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.Compiler.SyntaxAnalysis;
using static P4_Project.TypeS;

namespace P4_Project.SymTab
{
    public class SymbolTable
    {
        Parser parser;
        public static Obj undefObj = new Obj("undef", undef, var);

        // Kinds
        public const int var = 0, func = 1;

        SymbolTable parent;
        List<SymbolTable> innerScopes = new List<SymbolTable>();
        HashSet<Obj> symbolDecls = new HashSet<Obj>();

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

        public Obj NewObj(Obj obj)
        {
            return NewObj(obj.Name, obj.Kind, obj.Type);
        }

        //creates a new Object in the current scope
        public Obj NewObj(string name, int type, int kind)
        {
            Obj obj = new Obj(name, type, kind);

            if (symbolDecls.Contains(obj))
                parser.SemErr($"{name} declared twice");
            else
                symbolDecls.Add(obj);

            return obj;
        }

        //search for a name in all open scopes and return its object node
        public Obj Find(string name)
        {
            return undefObj;
        }

    }
}
