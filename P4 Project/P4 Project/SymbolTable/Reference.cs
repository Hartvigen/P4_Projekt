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

        /*enters name in the symbol table’s current scope.
        The parameter type conveys the data type and access attributes of name’s
        declaration.*/
        void EnterSymbol(String name, Type type)
        {

        }

        /*returns the symbol table’s currently valid declaration
        for name. If no declaration for name is currently in effect, then a null
        pointer is returned.*/

        object RetrieveSymbol(String name)
        {
            object sym = test.hashtable[name];
            while (sym != null)
            {
                if (sym.GetType().GetProperty("Name").GetValue(this).ToString() == name)
                    return sym;
                sym = sym.GetHashCode(); //Default hash function. (maybe change later?)
            }
            return null;
        }

    /*tests whether name is present in the symbol table’s
    current (innermost) scope. If it is, true is returned. If name is in an*/
        Boolean DeclaredLocally(String name)
        {
            return false;
        }

    }
}
