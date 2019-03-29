using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.SymbolTable
{
    class SymbolTable
    {
        const int undef = 0, number = 1, boolean = 2, text = 3, vertex = 4, edge = 5, set = 10, list = 20, queue = 30, stack = 40;

        const int var = 0, proc = 1, scope = 2;

        public int curLevel;
        public Obj undefObj;
        public Obj topScope;

        Compiler.SyntaxAnalysis.Parser parser;

        //Constructor for visitor (no parser argument)
        public SymbolTable()
        {
            topScope = null;
            curLevel = -1;
            undefObj = new Obj("undef", var, null, 0, undef, 0);
        }

        //Constructor for ATG
        public SymbolTable(Compiler.SyntaxAnalysis.Parser parser)
        {
            this.parser = parser;
            topScope = null;
            curLevel = -1;
            undefObj = new Obj("undef", var, null, 0, undef, 0);
        }

        //open a new scope and make it the current (topScope)
        void OpenScope()
        {
            Obj scop = new Obj("", scope, null, topScope, 0);
            topScope = scop;
            curLevel++;
        }

        //close the current scope
        void CloseScope()
        {
            topScope = topScope.Next;
            curLevel--;
        }

        //creates a new Object in the current scope
        public Obj NewObj(string name, int kind, int type)
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
