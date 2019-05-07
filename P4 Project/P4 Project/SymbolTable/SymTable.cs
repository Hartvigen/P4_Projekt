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
        private SymTable Parent { get; set; }
        private List<SymTable> InnerScopes { get; } = new List<SymTable>();
        private readonly Dictionary<string, Obj> _symbolDecls = new Dictionary<string, Obj>();

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



        //search for a name in all open scopes and return its object node
        public Obj Find(string name)
        {
            if (_symbolDecls.TryGetValue(name, out var value))
                return value;

            if (Parent != null && Parent.name != "top")
                return Parent.Find(name);

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

        public void PrintAllInCurrentScope()
        {
            if (name != "")
            {
                Console.WriteLine("Named scope: " + name);
            }
            foreach (var keyValuePair in _symbolDecls)
            {
                Console.WriteLine("    Name: " + keyValuePair.Key + " Type: " + keyValuePair.Value.type);
            }
            if(Parent !=  null)
            Parent.PrintAllInCurrentScope();
        }
        
        public void PrintAllInAllScopes()
        {
                InnerScopes.ForEach(i =>
                {
                    i.PrintAllInCurrentScope();
                });
        }

        public BaseType findReturnTypeOfFunction(string name)
        {
            if (IsPreDefFunctions(name))
                return FindReturnOfPreDefFunctions(name);
            
            if (_symbolDecls.ContainsKey(name))
            {
                _symbolDecls.TryGetValue(name, out Obj value);
                return value.type;
            }else throw new Exception("Name " + name + " does not belong to a function.");
        }

        public bool FunctionExists(string name)
        {
            return (IsPreDefFunctions(name) || _symbolDecls.ContainsKey(name));
        }

        public List<BaseType> findParameterListOfFunction(string name)
        {
            if (IsPreDefFunctions(name)) return FindParameterListOfPreDefFunctions(name);
            if (_symbolDecls.ContainsKey(name))
            {
                _symbolDecls.TryGetValue(name, out Obj value);
                return value.type.parameterTypes;
            }else throw new Exception("Name " + name + " does not belong to a function.");
        }
        private bool IsPreDefFunctions(string name)
        {
            switch (name) { 
                case "GetEdge": return true;
                case "RemoveEdge": return true;
                case "GetEdges": return true;
                case "RemoveVertex": return true;
                case "GetVertices": return true;
                case "ClearEdges": return true;
                case "ClearVertices": return true;
                case "ClearAll": return true;
                default: return false;
            }
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
                case "ClearVertices": return new BaseType("none");
                case "ClearAll": return new BaseType("none");
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }

        private List<BaseType> FindParameterListOfPreDefFunctions(string name)
        {
            switch (name)
            {
                case "GetEdge": return new List<BaseType> {new BaseType("vertex"), new BaseType("vertex")};
                case "RemoveEdge": return new List<BaseType> {new BaseType("vertex"), new BaseType("vertex")};
                case "RemoveVertex": return new List<BaseType> { new BaseType("vertex") };
                case "GetEdges": return new List<BaseType>();
                case "GetVertices": return new List<BaseType>();
                case "ClearEdges": return new List<BaseType>();
                case "ClearVertices": return new List<BaseType>();
                case "ClearAll": return new List<BaseType>();
                default: throw new Exception("the function: " + name + " is not a predefined function");
            }
        }
    }
}
