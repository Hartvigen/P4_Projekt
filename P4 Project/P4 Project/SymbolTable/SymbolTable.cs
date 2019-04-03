using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4_Project.Compiler.SyntaxAnalysis;
using static P4_Project.Types;

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
        List<Obj> symbolDecls = new List<Obj>();

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
            Obj p, last, obj = new Obj();
            obj.Name = name; obj.Kind = kind; obj.Type = type;
            obj.Level = curLevel;

            p = topScope.Locals;
            last = null;

            while (p != null)
            {
                if (p.Name == name)
                    parser.SemErr("name declared twice");
                last = p;
                p = p.Next;
            }

            if (last == null)
                topScope.Locals = obj;
            else
                last.Next = obj;

            if (kind == var)
                obj.Adr = topScope.NextAdr++;

            return obj;
        }

        //search for a name in all open scopes and return its object node
        public Obj Find(string name)
        {
            Obj obj, scope;
            scope = topScope;
            while(scope != null)
            {
                obj = scope.Locals;
                while(obj != null)
                {
                    if(obj.Name == name)
                    {
                        return obj;
                    }
                    obj = obj.Next;
                }
                scope = scope.Next;
            }
            parser.SemErr(name + " is undeclared");
            return undefObj;
        }

    }
}
