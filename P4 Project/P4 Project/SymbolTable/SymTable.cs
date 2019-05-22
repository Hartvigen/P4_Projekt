using System;
using System.Collections.Generic;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.AST;

namespace P4_Project.SymbolTable
{
    /// <summary>
    /// The SymTable class serves a lot of different Roles.
    /// 1. Scope relative position is encoded in the parent system.
    /// 2. variables contains attributes from vertex and edge.
    /// 3. Functions are stored here.
    /// 4. Exposes functions that are used generally throughout the visitors and Interpreter.
    /// </summary>
    public class SymTable
    {
        /// <summary>
        /// The Parser is the common link between the Visitors and scopes.
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Name of the scope is either used to indicate that this scope if the "top" scope or that is a functionscope
        /// In which case the name is the name of the function.
        /// </summary>
        public string name;

        public const int Var = 0, Func = 1; // Kinds

        /// <summary>
        /// The Header bool  is set true if the symTable is a header 
        ///and can thus easily be detected and deleted when not needed.
        /// </summary>
        public bool header;
        
        /// <summary>
        /// The Parent of the Scope.
        /// </summary>
        private SymTable Parent { get; }
        
        /// <summary>
        /// The InnerScopes of the Scope.
        /// </summary>
        private List<SymTable> InnerScopes { get; } = new List<SymTable>();
        
        /// <summary>
        /// Variables contain variables from this exact scope.
        /// </summary>
        private readonly Dictionary<string, Obj> _variables = new Dictionary<string, Obj>();

        /// <summary>
        /// SymbolTables that contain variables that are actually the attributes of vertex and edge respectively.
        /// </summary>
        public SymTable vertexAttr;
        public SymTable edgeAttr;

        /// <summary>
        /// Counts one up every time we go one more forward in scopes.
        /// </summary>
        private int _position;

        /// <summary>
        /// The Scope can have a Type if the scope is function scope the type will contain the function return type.
        /// </summary>
        public BaseType type;

        /// <summary>
        /// Constructor for tables with no name
        /// </summary>
        /// <param name="parent">Parent of the scope</param>
        /// <param name="parser">Parser</param>
        public SymTable(SymTable parent, Parser parser)
        {
            Parent = parent;
            _parser = parser;
            name = "";
        }
        
        /// <summary>
        /// Constructor for Function scopes where the name is the function name
        /// </summary>
        /// <param name="parent">Parent of the scope</param>
        /// <param name="parser">Parser</param>
        /// <param name="name">Name of the function</param>
        public SymTable(SymTable parent, Parser parser, string name)
        {
            Parent = parent;
            _parser = parser;
            this.name = name;
        }


        /// <summary>
        /// Adds another scope to InnerScopes with this scope as its parent.
        /// </summary>
        /// <returns>Returns the new InnerScope.</returns>
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

        /// <summary>
        /// Closes current scope
        /// </summary>
        /// <returns>Returns this scopes parent as a way of closing the scope.</returns>
        public SymTable CloseScope()
        {
            return Parent;
        }

        /// <summary>
        /// Creates a new Var from information and adds it to this scope.
        /// </summary>
        /// <param name="objName">Name of the var</param>
        /// <param name="t">Type of the var</param>
        /// <param name="kind">Kind of the var (Almost never used just set 1 or 0)</param>
        /// <returns>Returns the var after it was created and added to the scope.</returns>
        public Obj NewVar(string objName, BaseType t, int kind)
        {
            var obj = new Obj(objName, t, kind);
            AddVar(obj);
            return obj;
        }

        /// <summary>
        /// Adds a new Var to this exact scope.
        /// </summary>
        /// <param name="obj">Obj of var to be added</param>
        /// <exception cref="Exception">Throws and Exception if the Var is already present in this scope.</exception>
        public void AddVar(Obj obj)
        {
            if (!_variables.ContainsKey(obj.Name))
                _variables.Add(obj.Name, obj);
            
        }

        /// <summary>
        /// Removes a var from this scope only if it is exactly in this scope.
        /// </summary>
        /// <param name="obj">obj of the var to be Removed.</param>
        /// <exception cref="Exception">Throws an Exception if var is not present in this scope.</exception>
        public void RemoveVar(Obj obj)
        {
            if (!_variables.ContainsKey(obj.Name))
                throw new Exception("Cannot remove Obj: " + obj.Name + " as it doesnt exist in this scope");
            _variables.Remove(obj.Name);
        }

        /// <summary>
        /// Searches for a Var in this if not found it will ask a parent if the parent is not null
        /// This will essentially search for a var and find it if it is reachable from this scope.
        /// </summary>
        /// <param name="VarName"></param>
        /// <returns>Returns the Obj information of the Var</returns>
        public Obj FindVar(string VarName)
        {
            return _variables.TryGetValue(VarName, out var value) ? value : (Parent?.FindVar(VarName) ?? null);
        }

        /// <summary>
        /// Returns all InnerScopes
        /// </summary>
        /// <returns>Returns All Innerscopes</returns>
        public List<SymTable> GetInnerScopes()
        {
            return InnerScopes;
        }

        /// <summary>
        /// Returns the Variables in this scope, not all variables visible in the scope or variables from innerScopes.
        /// Only variables in this exact scope.
        /// </summary>
        /// <returns>Returns a dictionary mapping variable names to Obj</returns>
        public Dictionary<string, Obj> GetVariables()
        {
            return _variables;
        }

        /// <summary>
        /// Will find the return type of a function Predefined or userDefined alike.
        /// </summary>
        /// <param name="functionName">Name of the function to find return type of</param>
        /// <param name="parameters">Parameter list (Can be null if the function is not predefined)</param>
        /// <returns>Returns the BaseType that corresponds to the return type</returns>
        /// <exception cref="Exception">Throws and Exception if the name does not belong to a function.</exception>
        public BaseType FindReturnTypeOfFunction(string functionName, List<BaseType> parameters)
        {
            if (PreDefined.PreDefinedFunctions.Contains(functionName))
                return PreDefined.FindReturnTypeOfPreDefFunctions(functionName, parameters);

            foreach (var s in InnerScopes) {
                if(s.name == functionName)
                    return s.type.returnType;
            }

            throw new Exception("Name " + name + " does not belong to a function.");
        }

        /// <summary>
        /// Will check if a function exists.
        /// </summary>
        /// <param name="functionName">The name of function to look for.</param>
        /// <returns>Returns true if Function is found, false otherwise.</returns>
        public bool FunctionExists(string functionName)
        {
            return PreDefined.PreDefinedFunctions.Contains(functionName) || InnerScopes.Exists(scp => scp.name == functionName);
        }

        /// <summary>
        /// Function can find the parameterList of any function Predefined or userDefined.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <returns>A List of Parameter options</returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<List<BaseType>> FindParameterListOfFunction(string functionName)
        {
            if (PreDefined.PreDefinedFunctions.Contains(functionName)) 
                return PreDefined.FindListOfParameterLists(functionName);

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

        /// <summary>
        /// It will enter the next scope by returning the scope from innerScopes depending on position.
        /// </summary>
        /// <returns>Returns the just entered scope.</returns>
        /// <exception cref="Exception">Throws and exception if more scopes are requested than are present.</exception>
        public SymTable EnterNextScope() {
            _position++;
            //If we have reached a position higher than their are scopes available we it must be an error!
            if (_position > InnerScopes.Count)
                throw new Exception("Ran out of scopes");
            return InnerScopes[_position - 1];
        }

        /// <summary>
        /// Returns true if the given attribute exists.
        /// </summary>
        /// <param name="typeName">Name of the type the attribute belongs to</param>
        /// <param name="attrName">Name of the attribute it self.</param>
        /// <returns>Returns true if attribute is found</returns>
        public bool IsAttribute(string typeName, string attrName) {
            switch (typeName)
            {
                case "vertex":
                    return vertexAttr._variables.ContainsKey(attrName);
                case "edge":
                    return edgeAttr._variables.ContainsKey(attrName);
                default: return false;
            }
        }

        /// <summary>
        /// The function will retrieve the BaseType of the attribute provided.
        /// </summary>
        /// <param name="typeName">The name of the underlying type</param>
        /// <param name="attrName">The name of the attribute we want the type of</param>
        /// <returns>Returns a BaseType of the same type as the attribute with name attrName that exists in type with name typeName </returns>
        /// <exception cref="Exception">Will throw an exception is the attribute cannot be found</exception>
		public BaseType GetTypeOfAttribute(string typeName, string attrName) {
            switch (typeName)
            {
                case "vertex":
                {
                    if (!IsAttribute(typeName, attrName)) throw new Exception(typeName + " can hold attributes but " + attrName + " is not a attribute if that type.");
                    vertexAttr._variables.TryGetValue(attrName, out var o);
                    return o?.Type;
                }

                case "edge":
                {
                    if (!IsAttribute(typeName, attrName)) throw new Exception(typeName + " can hold attributes but " + attrName + " is not a attribute if that type.");
                    edgeAttr._variables.TryGetValue(attrName, out var o);
                    return o?.Type;
                }
                default: throw new Exception(typeName + " is not a type that can hold attributes (only vertex and edge can!)");
            }
        }

        /// <summary>
        /// It will reset this SymTables scope position as well as make all its innerScopes do the same recursively.
        /// </summary>
		public void ResetScopePositions() {
            _position = 0;
            InnerScopes.ForEach(s => s.ResetScopePositions());
        }

        /// <summary>
        /// Removes the TopHeaderScope use in parser only.
        /// </summary>
        public void RemoveTopHeaderScope() {
            InnerScopes.RemoveAll(scp => scp.header);
        }
    }
}
