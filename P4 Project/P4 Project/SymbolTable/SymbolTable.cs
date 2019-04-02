using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P4_Project.Types;

namespace P4_Project.SymbolTable
{
    class SymbolTable
    {
        public const int var = 0, func = 1, scope = 2;

        public int curLevel;
        public Obj undefObj;
        public Obj topScope;

        Compiler.SyntaxAnalysis.Parser parser;
        


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

        //Constructor for the symbol table
        public SymbolTable(Compiler.SyntaxAnalysis.Parser parser)
        {
            this.parser = parser;
            topScope = null;
            curLevel = -1;
            undefObj = new Obj("undef", var, null, 0, undef, 0);
        }

    }
}
