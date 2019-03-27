using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.SymbolTable
{
    class Reference
    {
        private int _depth = 1;
        private Symbol _prevSym;
        private Symbol _nextSym;
        private SymbolTableClass test = new SymbolTableClass();
        private List<Symbol> ScopeDisplay = new List<Symbol>();
        
        /*tests whether name is present in the symbol table’s
        current (innermost) scope. If it is, true is returned. If name is in an*/


        void OpenScope()
        {
            _depth++;

        }

        /*closes the most recently opened scope in the symbol table.
        Symbol references subsequently revert to outer scopes*/
        void CloseScope()
        {
            foreach (Symbol s in ScopeDisplay)
            {
                _prevSym = s.var;
                DeleteSymbol(s);
                if (_prevSym == null)
                    AddSymbol(_prevSym);
            }
            _depth--;
        }


        /*returns the symbol table’s currently valid declaration
        for name. If no declaration for name is currently in effect, then a null
        pointer is returned.*/

        object RetrieveSymbol(String name)
        {
            object sym = test.hashtable[name];
            while (sym != null)
            {
                if (sym.GetType().GetProperty("name").GetValue(this).ToString() == name)
                    return sym;
                sym = sym.GetHashCode(); //Default hash function. (maybe change later?)
            }
            return null;
        }


        /*enters name in the symbol table’s current scope.
        The parameter type conveys the data type and access attributes of name’s
        declaration.*/
        void EnterSymbol(String name, Type type)
        {
            object oldsym = RetrieveSymbol(name);
            if(oldsym != null && Convert.ToInt32(oldsym.GetType().GetProperty("depth").GetValue(this)) == 1) //replace 1 with current depth level.
                {}// Call error("Dublicate definition of " + name);
            object newsym = CreateNewSymbol(name, type); //Create CreateNewSymbol();
            // add to scopedisplay
            newsym.GetType().GetProperty("level").SetValue(this, 1); //replace 1 with scopeDisplay[depth]
            newsym.GetType().GetProperty("depth").SetValue(this, 1); //replace 1 with current depth level.
            //scopeDisplay[depth] <- newsym
            //add to hash table
            if (oldsym == null)
                AddSymbol(oldsym);
            else
            {
                DeleteSymbol(oldsym);
                AddSymbol(newsym);
            }
            newsym.SetVar(oldsym);  //newsym.var <- oldsym (Not sure)
        }

        /*delete(sym) removes the symbol table entry sym from the collision chain
        found at HashTable.get(sym.name). The symbol is not destroyed—it is
        simply removed from the collision chain.In particular, its var and level
        fields remain intact.
        add(sym) addsthe symbol sym tothe collision chain at HashTable.get(sym.name).
        Prior to the call to add, there is no entry in the table for sym.*/
        private void AddSymbol(object newsym)
        {
            throw new NotImplementedException();
        }

        private void DeleteSymbol(object oldsym)
        {
            throw new NotImplementedException();
        }

        private object CreateNewSymbol(string name, Type type)
        {
            throw new NotImplementedException();
        }

        /*tests whether name is present in the symbol table’s
        current (innermost) scope. If it is, true is returned. If name is in an*/
        Boolean DeclaredLocally(String name)
        {
            return false;
        }

    }
}
