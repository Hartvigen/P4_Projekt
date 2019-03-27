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
        SymbolTableClass test = new SymbolTableClass();
        
        /*tests whether name is present in the symbol table’s
        current (innermost) scope. If it is, true is returned. If name is in an*/


        void OpenScope()
        {

        }

        /*closes the most recently opened scope in the symbol table.
        Symbol references subsequently revert to outer scopes*/
        void CloseScope()
        {
           
        }


        /*returns the symbol table’s currently valid declaration
        for name. If no declaration for name is currently in effect, then a null
        pointer is returned.*/

        object RetrieveSymbol(string name)
        {
            Symbol sym = new Symbol(test.hashtable[name]);

            while (sym != null)
            {
                if (sym.GetName() == name)
                    return sym;
                sym.SetHash(name.GetHashCode().ToString()); //Default hash function. (maybe change later?)
            }
            return null;
        }


        /*enters name in the symbol table’s current scope.
        The parameter type conveys the data type and access attributes of name’s
        declaration.*/
        void EnterSymbol(string name, Type type)
        {
            Symbol oldsym = new Symbol(RetrieveSymbol(name));
            if(oldsym != null && oldsym.GetDepth() == 1) //replace 1 with current depth level.
                {}// Call error("Dublicate definition of " + name);
            Symbol newsym = new Symbol(CreateNewSymbol(name, type)); //Create CreateNewSymbol();
            // add to scopedisplay
            newsym.SetLevel(1); //replace 1 with scopeDisplay[depth]
            newsym.SetDepth(1); //replace 1 with current depth level.
            //scopeDisplay[depth] <- newsym
            //add to hash table
            if (oldsym == null)
                AddSymbol(oldsym);
            else
            {
                DeleteSymbol(oldsym);
                AddSymbol(newsym);
            }
            newsym.SetVar(oldsym.GetVar());  //newsym.var <- oldsym (Not sure)
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
        Boolean DeclaredLocally(string name)
        {
            return false;
        }

    }
}
