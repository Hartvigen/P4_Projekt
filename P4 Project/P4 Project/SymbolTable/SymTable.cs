using System;
using System.Collections.Generic;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.AST;

namespace P4_Project.SymbolTable
{
    public class SymTable
    {
        private readonly Parser parser;

        public string name;

        public const int Var = 0, Func = 1; // Kinds

        public bool header;
        private SymTable Parent { get; }
        private List<SymTable> InnerScopes { get; } = new List<SymTable>();
        private readonly Dictionary<string, Obj> variables = new Dictionary<string, Obj>();

        public SymTable vertexAttr;
        public SymTable edgeAttr;

        //Used to keep track of the next innerScope
        private int position;

        public BaseType type;

        //Constructor for visitor (no parser argument)
        public SymTable(SymTable parent, Parser parser)
        {
            Parent = parent;
            this.parser = parser;
            name = "";
        }
        
        public SymTable(SymTable parent, Parser parser, string name)
        {
            Parent = parent;
            this.parser = parser;
            this.name = name;
        }


        //open a new scope and make it the current (topScope)
        public SymTable OpenScope()
        {
            SymTable newTable = new SymTable(this, parser);
            InnerScopes.Add(newTable);
            return newTable;
        }
        
        public SymTable OpenScope(string Name)
        {
            SymTable newTable = new SymTable(this, parser, Name);
            InnerScopes.Add(newTable);
            return newTable;
        }

        //close the current scope
        public SymTable CloseScope()
        {
            return Parent;
        }

        //creates a new Object in the current scope
        public Obj NewObj(string Name, BaseType t, int kind)
        {
            var obj = new Obj(Name, t, kind);

            if (!variables.ContainsKey(obj.Name))
                variables.Add(obj.Name, obj);
            else
            {
                Console.WriteLine(obj.Name + " is already added to symDecls");
                parser.SemErr(obj.Name + " has already been declared");
            }
            return obj;
        }

        //Adds a new Object in the current scope
        public void AddObj(Obj obj)
        {
            if (!variables.ContainsKey(obj.Name))
                variables.Add(obj.Name, obj);
            else Console.WriteLine(obj.Name + " is already added to symDecls");
        }

        //Removes a Object in the current scope
        public void RemoveObj(Obj obj)
        {
            if (!variables.ContainsKey(obj.Name)) {
                Console.WriteLine("Cannot remove Obj: " + obj.Name + " as it doesnt exist in this dictionary");
                return;
            }
            variables.Remove(obj.Name);
        }

        //search for a name in all open scopes and return its object node
        public Obj Find(string Name)
        {
            if (variables.TryGetValue(Name, out var value))
                return value;

            //We only ask parent if it is not the scope named top that also has a null parent as that would mean it was a true top scope.
            //This allows for functions to actually be named "top" as their otherwise could have been problems with that.
            if (Parent == null) return null;
            if (Parent.name == "top" && Parent.Parent == null)
                return null;
            return Parent.Find(Name);
        }

        // return all the innerScopes
        public List<SymTable> GetScopes()
        {
            return InnerScopes;
        }

        // return the dictionary
        public Dictionary<string, Obj> GetDic()
        {
            return variables;
        }

        public BaseType findReturnTypeOfFunction(string Name)
        {
            if (PreDefined.preDefinedFunctions.Contains(Name))
                return PreDefined.FindReturnOfPreDefFunctions(Name);
            foreach (var s in InnerScopes) {
                if(s.name == Name)
                    return s.type;
            }
            throw new Exception("Name " + name + " does not belong to a function.");
        }

        public bool FunctionExists(string Name)
        {
            return PreDefined.preDefinedFunctions.Contains(Name) || variables.ContainsKey(Name);
        }

        public List<BaseType> findParameterListOfFunction(string Name)
        {
            if (PreDefined.preDefinedFunctions.Contains(Name)) return PreDefined.FindParameterListOfPreDefFunctions(Name);
            //Complicated piece of code that can either find the function in its proper place or in variables because function is used in the cleaner before functions gets cleaned up.
            foreach (var s in InnerScopes)
            {
                if (s.name != Name) continue;
                if (s.type != null) return s.type.parameterTypes;
                variables.TryGetValue(Name, out var o);
                if (o != null) return o.Type.parameterTypes;
            }
            throw new Exception("Name " + Name + " does not belong to a function.");
        }

        public SymTable EnterNextScope() {
            position++;
            //If we have reached a position higher than their are scopes available we it must be an error!
            if (position > InnerScopes.Count)
                throw new Exception("Ran out of scopes");
            return InnerScopes[position - 1];
        }

        public bool isAttribute(string typeName, string attrName) {
            switch (typeName)
            {
                case "vertex":
                    return vertexAttr.variables.ContainsKey(attrName);
                case "edge":
                    return edgeAttr.variables.ContainsKey(attrName);
                default:
                    throw new Exception(typeName + " is not possible to hold attributes");
            }
        }

		public BaseType getTypeOfAttribute(string typeName, string attrName) {
            if (!isAttribute(typeName, attrName)) throw new Exception(typeName + " is not possible to hold attributes");
            switch (typeName)
            {
                case "vertex":
                {
                    vertexAttr.variables.TryGetValue(attrName, out var o);
                    return o?.Type;
                }

                case "edge":
                {
                    edgeAttr.variables.TryGetValue(attrName, out var o);
                    return o?.Type;
                }

                default:
                    throw new Exception(typeName + " is not possible to hold attributes");
            }
        }


		public void resetScopePositions() {
            position = 0;
            InnerScopes.ForEach(s => s.resetScopePositions());
        }
    }
}
