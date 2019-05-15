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
        private SymTable Parent { get; set; }
        private List<SymTable> InnerScopes { get; } = new List<SymTable>();
        private Dictionary<string, Obj> _symbolDecls = new Dictionary<string, Obj>();

        public SymTable vertexAttr;
        public SymTable edgeAttr;

        //Used to keep track of the next Innerscope
        private int position = 0;

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
            SymTable newTable = new SymTable(this, _parser);
            InnerScopes.Add(newTable);
            return newTable;
        }
        
        public SymTable OpenScope(string name)
        {
            SymTable newTable = new SymTable(this, _parser, name);
            InnerScopes.Add(newTable);
            return newTable;
        }

        //close the current scope
        public SymTable CloseScope()
        {
            return Parent;
        }

        //creates a new Object in the current scope
        public Obj NewObj(string name, BaseType type, int kind, SymTable scope = null)
        {
            var obj = new Obj(name, type, kind, scope);

            if (!_symbolDecls.ContainsKey(obj.Name))
                _symbolDecls.Add(obj.Name, obj);
            else Console.WriteLine(obj.Name + " is already added to symDecls");
            return obj;
        }

        //Adds a new Object in the current scope
        public void AddObj(Obj obj)
        {
            if (!_symbolDecls.ContainsKey(obj.Name))
                _symbolDecls.Add(obj.Name, obj);
            else Console.WriteLine(obj.Name + " is already added to symDecls");
        }

        //Removes a Object in the current scope
        public void RemoveObj(Obj obj)
        {
            if (!_symbolDecls.ContainsKey(obj.Name)) {
                Console.WriteLine("Cannot remove Obj: " + obj.Name + " as it doesnt exist in this dictionary");
                return;
            }
            _symbolDecls.Remove(obj.Name);
        }

        //search for a name in all open scopes and return its object node
        public Obj Find(string name)
        {
            if (_symbolDecls.TryGetValue(name, out var value))
                return value;

            //We only ask parent if it is not the scope named top that also has a null parent as that would mean it was a true top scope.
            //This allows for functions to actually be named "top" as their otherwise could have been problems with that.
            if (Parent != null)
            {
                if (Parent.name == "top" && Parent.Parent == null)
                        return null;
                return Parent.Find(name);
            }
            return null;
        }

        // return all the innerScopes
        public List<SymTable> GetScopes()
        {
            return InnerScopes;
        }

        // return the dictionary
        public Dictionary<string, Obj> GetDic()
        {
            return _symbolDecls;
        }

        public BaseType findReturnTypeOfFunction(string name)
        {
            if (IsPreDefFunctions(name))
                return FindReturnOfPreDefFunctions(name);
            foreach (var s in InnerScopes) {
                if(s.name == name)
                    return s.type;
            }
            throw new Exception("Name " + name + " does not belong to a function.");
        }

        public bool FunctionExists(string name)
        {
            return (IsPreDefFunctions(name) || _symbolDecls.ContainsKey(name));
        }

        public List<BaseType> findParameterListOfFunction(string name)
        {
            if (IsPreDefFunctions(name)) return FindParameterListOfPreDefFunctions(name);
            //Complicated piece of code that can either find the function in its propper place or in symbdecls becuase function is used in the cleaner before functions gets cleaned up.
            foreach (var s in InnerScopes)
            {
                if (s.name == name)
                    if (s.type == null) {
                        _symbolDecls.TryGetValue(name, out Obj o);
                        return o.Type.parameterTypes;
                    }else return s.type.parameterTypes;
            }
            throw new Exception("Name " + name + " does not belong to a function.");
        }
        private bool IsPreDefFunctions(string name)
        {
            return PreDefined.preDefinedFunctions.Contains(name);
        }

        private BaseType FindReturnOfPreDefFunctions(string name)
        {
            switch (name)
            {
                case "GetEdge": return new BaseType("edge");
                case "RemoveEdge": return new BaseType("none");
                case "GetEdges": return new BaseType(new BaseType("set"), new BaseType("edge"));
                case "RemoveVertex": return new BaseType("none");
                case "GetVertices": return new BaseType(new BaseType("set"), new BaseType("vertex"));
                case "ClearEdges": return new BaseType("none");
                case "ClearAll": return new BaseType("none");
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }

        private List<BaseType> FindParameterListOfPreDefFunctions(string name)
        {
            switch (name)
            {
                case "GetEdge": return new List<BaseType> {new BaseType("vertex"), new BaseType("vertex")};
                case "RemoveEdge": return new List<BaseType> {new BaseType("edge")};
                case "RemoveVertex": return new List<BaseType> { new BaseType("vertex") };
                case "GetEdges": return new List<BaseType>();
                case "GetVertices": return new List<BaseType>();
                case "ClearEdges": return new List<BaseType>();
                case "ClearAll": return new List<BaseType>();
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }

        public SymTable EnterNextScope() {
            position++;
            //If we have reached a position higher than their are scopes availble we it must be an error!
            if (position > InnerScopes.Count)
                throw new Exception("Ran out of scopes");
            return InnerScopes[position - 1];
        }

        public bool isAttribute(string typeName, string attrName) {
            bool found = false;
            if (typeName == "vertex")
                return vertexAttr._symbolDecls.ContainsKey(attrName);
            else if(typeName == "edge")
                return edgeAttr._symbolDecls.ContainsKey(attrName);
            throw new Exception(typeName + " is not possible to hold attributes");
        }

		public BaseType getTypeOfAttribute(string typeName, string attrName) {
			if (isAttribute(typeName, attrName)) {
				if (typeName == "vertex")
				{
					vertexAttr._symbolDecls.TryGetValue(attrName, out Obj o);
					return o.Type;
				}
				else if (typeName == "edge")
				{
					edgeAttr._symbolDecls.TryGetValue(attrName, out Obj o);
					return o.Type;
				}
			}
			throw new Exception(typeName + " is not possible to hold attributes");
		}


		public void resetScopePositions() {
            position = 0;
            InnerScopes.ForEach(s => s.resetScopePositions());
        }
    }
}
