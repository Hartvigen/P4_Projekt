using System;
using System.Collections.Generic;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.AST;

namespace P4_Project.SymbolTable
{
    public class SymTable
    {
        private readonly Parser _parser;

        public string name;

        public const int Var = 0, Func = 1; // Kinds

        public bool header;
        private SymTable Parent { get; }
        private List<SymTable> InnerScopes { get; } = new List<SymTable>();
        private readonly Dictionary<string, Obj> _variables = new Dictionary<string, Obj>();

        public SymTable vertexAttr;
        public SymTable edgeAttr;

        //Used to keep track of the next innerScope
        private int _position;

        public BaseType type;

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
            var newTable = new SymTable(this, _parser);
            InnerScopes.Add(newTable);
            return newTable;
        }
        
        /// <summary>
        /// This Constructor is used in the parser to give a function scope a name corresponding the function name.
        /// </summary>
        /// <param name="funcName">The function name</param>
        /// <returns>Returns a the scope</returns>
        public SymTable OpenScope(string funcName)
        {
            var newTable = new SymTable(this, _parser, funcName);
            InnerScopes.Add(newTable);
            return newTable;
        }

        //close the current scope
        public SymTable CloseScope()
        {
            return Parent;
        }

        //creates a new Object in the current scope
        public Obj NewObj(string objName, BaseType t, int kind)
        {
            var obj = new Obj(objName, t, kind);

            if (!_variables.ContainsKey(obj.Name))
                _variables.Add(obj.Name, obj);
            else
            {
                Console.WriteLine(obj.Name + " is already added to symDecls");
                _parser.SemErr(obj.Name + " has already been declared");
            }
            return obj;
        }

        //Adds a new Object in the current scope
        public void AddObj(Obj obj)
        {
            if (!_variables.ContainsKey(obj.Name))
                _variables.Add(obj.Name, obj);
            else Console.WriteLine(obj.Name + " is already added to symDecls");
        }

        //Removes a Object in the current scope
        public void RemoveObj(Obj obj)
        {
            if (!_variables.ContainsKey(obj.Name)) {
                Console.WriteLine("Cannot remove Obj: " + obj.Name + " as it doesnt exist in this dictionary");
                return;
            }
            _variables.Remove(obj.Name);
        }

        //search for a name in all open scopes and return its object node
        public Obj Find(string objName)
        {
            return _variables.TryGetValue(objName, out var value) ? value : Parent.Find(objName);
        }

        // return all the innerScopes
        public List<SymTable> GetScopes()
        {
            return InnerScopes;
        }

        // return the dictionary
        public Dictionary<string, Obj> GetDic()
        {
            return _variables;
        }

        public BaseType FindReturnTypeOfFunction(string functionName)
        {
            if (PreDefined.PreDefinedFunctions.Contains(functionName))
                return PreDefined.FindReturnOfPreDefFunctions(functionName);
            foreach (var s in InnerScopes) {
                if(s.name == functionName)
                    return s.type;
            }
            throw new Exception("Name " + name + " does not belong to a function.");
        }

        public bool FunctionExists(string functionName)
        {
            return PreDefined.PreDefinedFunctions.Contains(functionName) || _variables.ContainsKey(functionName);
        }

        public List<List<BaseType>> FindParameterListOfFunction(string functionName)
        {
            if (PreDefined.PreDefinedFunctions.Contains(functionName)) return PreDefined.FindListOfParameterLists(functionName);
            //Complicated piece of code that can either find the function in its proper place or in variables because function is used in the cleaner before functions gets cleaned up.
            foreach (var s in InnerScopes)
            {
                if (s.name != functionName) continue;
                if (s.type != null) return new List<List<BaseType>>{s.type.parameterTypes};
                _variables.TryGetValue(functionName, out var o);
                if (o != null) return new List<List<BaseType>>{o.Type.parameterTypes};
            }
            throw new Exception("Name " + functionName + " does not belong to a function.");
        }

        public SymTable EnterNextScope() {
            _position++;
            //If we have reached a position higher than their are scopes available we it must be an error!
            if (_position > InnerScopes.Count)
                throw new Exception("Ran out of scopes");
            return InnerScopes[_position - 1];
        }

        public bool IsAttribute(string typeName, string attrName) {
            switch (typeName)
            {
                case "vertex":
                    return vertexAttr._variables.ContainsKey(attrName);
                case "edge":
                    return edgeAttr._variables.ContainsKey(attrName);
                default:
                    throw new Exception(typeName + " is not possible to hold attributes");
            }
        }

		public BaseType GetTypeOfAttribute(string typeName, string attrName) {
            if (!IsAttribute(typeName, attrName)) throw new Exception(typeName + " is not possible to hold attributes");
            switch (typeName)
            {
                case "vertex":
                {
                    vertexAttr._variables.TryGetValue(attrName, out var o);
                    return o?.Type;
                }

                case "edge":
                {
                    edgeAttr._variables.TryGetValue(attrName, out var o);
                    return o?.Type;
                }

                default:
                    throw new Exception(typeName + " is not possible to hold attributes");
            }
        }


		public void ResetScopePositions() {
            _position = 0;
            InnerScopes.ForEach(s => s.ResetScopePositions());
        }
    }
}
