using System;
using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Interpreter.Types;
using P4_Project.Compiler.SemanticAnalysis.Visitors;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.SymbolTable;

namespace P4_Project.Compiler.Interpreter
{
    public sealed class Executor : Visitor
    {
        //This is the given information.
        //That will be used to execute the entire program.
        private SymTable Table { get; }

        //The Definition of a vertex and edge attributions.
        private readonly Dictionary<string, BaseType> _defVertexAttr = new Dictionary<string, BaseType>();
        private readonly Dictionary<string, BaseType> _defEdgeAttr = new Dictionary<string, BaseType>();

        //The Scene will be printed when a print statement is encountered.
        public readonly List<Vertex> scene = new List<Vertex>();

        //The functions
        private readonly Dictionary<string, FuncDeclNode> _functions = new Dictionary<string, FuncDeclNode>();

        //All the main nodes
        private readonly Scope _mainScope = new Scope(null);

        private readonly Stack<Scope> _callStack = new Stack<Scope>();

        private string _executionInterrupted;
        private const string ExeBreak = "break", ExeContinue = "continue", ExeReturn = "return";

        //Current scope used to keep track of the current scope at runtime
        private Scope _currentScope;
        internal Value currentValue;

        public override string AppropriateFileName { get; } = "CompileRes.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();

        public Executor(SymTable table)
        {
            Table = table;
            _currentScope = _mainScope;
            MoveAttrDefinitions();
        }

        private void MoveFunctions(Magia node)
        {
            foreach (var s in node.block.Statements) {
                if (s.GetType() != typeof(FuncDeclNode)) continue;
                var f = (FuncDeclNode)s;
                _functions.Add(f.SymbolObject.Name, f);
            }
        }
        private void CreateNewVertex(DeclNode vertex)
        {
            //The vertex is created
            var values = new Dictionary<string, Value>();
            foreach (var keyValuePair in _defVertexAttr) 
                values.Add(keyValuePair.Key, new Value(PreDefined.GetDefaultValueOfAttributeType(keyValuePair.Value)));
            var v = new Vertex(vertex, values);
            //We add it to the scene and the currentScope.
            scene.Add(v);
            _currentScope.CreateVar(v.identifier,new Value(v));
        }
        private void CreateNewEdge(EdgeCreateNode edge)
        {
            _currentScope.TryGetValue(edge.LeftSide.Ident, out var v);
            var from = (Vertex)v.o;
            foreach (var tuple in edge.RightSide) {
                var values = new Dictionary<string, Value>();
                foreach (var keyValuePair in _defEdgeAttr)
                    values.Add(keyValuePair.Key, new Value(PreDefined.GetDefaultValueOfAttributeType(keyValuePair.Value)));
                _currentScope.TryGetValue(tuple.Item1.Ident, out var v1);
                var to = (Vertex)v1.o;
                var e = new Edge(from, edge.Operator, to, values);
                foreach (var l in tuple.Item2)
                {
                    l.Value.Accept(this);
                    e.UpdateAttribute(l.Target.Ident, currentValue);
                }
                from.edges.Add(e);
            }
        }

        /// <summary>
        /// Function runs only once in the constructor of executor.
        /// Its response ability is to move attributes to into the executor from the Table.
        /// </summary>
        private void MoveAttrDefinitions()
        {
            if (Table.vertexAttr is null) return;
                foreach (var v in Table.vertexAttr.GetDic())
                    _defVertexAttr.Add(v.Key, v.Value.Type);
            
            if (Table.edgeAttr is null) return;
                foreach (var v in Table.edgeAttr.GetDic())
                    _defEdgeAttr.Add(v.Key, v.Value.Type);
        }

        public override void Visit(CallNode node)
        {
            //If it is preDefined we execute internally.
            if (PreDefined.PreDefinedFunctions.Contains(node.Ident))
            {
                var parameters = new List<Value>();
                node.Parameters.Expressions.ForEach(e =>
                {
                    e.Accept(this);
                    parameters.Add(currentValue);
                });
                PreDefined.DoPreDefFunction(node.Ident, this, parameters);
                return;
            }
            
            //If we cannot find it there was a call to a function that does not exist
            if (!_functions.TryGetValue(node.Ident, out var f))
                throw new Exception(node.Ident + " is not a predefined function and not a user defined function");
            
            //We create a new callScope on the call stack
            _callStack.Push(new Scope(_currentScope));

            //We add the parameters as variables in that scope using the retrieved function
            for (var i = f.Parameters.Statements.Count; i > 0; i--)
            {
                var v = (VarDeclNode) f.Parameters.Statements[i-1];
                node.Parameters.Expressions[i-1].Accept(this);
                _callStack.Peek().CreateVar(v.SymbolObject.Name, currentValue);
            }
            //We Visit that function to execute it
            f.Accept(this);

            _callStack.Pop();
            _currentScope = _callStack.Count == 0 ? _mainScope : _callStack.Peek();
        }

        public override void Visit(VarNode node)
        {
            if (node.Source == null)
            {
                _currentScope.TryGetValue(node.Ident, out Value value);
                currentValue = value;
            }
            else
            {
                var o = DecodeReference(node.Source); // Yes, we need the source, in order to change the attribute inside the source
                switch (o)
                {
                    case Vertex vertex:
                    {
                        var v = vertex;
                        currentValue = v.attributes[node.Ident];
                        break;
                    }

                    case Edge e:
                        currentValue = e.attributes[node.Ident];
                        break;
                    default:
                        throw new Exception($"Tried to access attribute {node.Ident} in type that is not vertex or edge.");
                }
            }

            if (currentValue == null)
                throw new Exception("current value cannot be null after var node");
        }

        public override void Visit(MultiDecl multiDecl)
        {
            multiDecl.Decls.ForEach(d => d.Accept(this));
        }



        public override void Visit(CollecConst node)
        {
            IEnumerable<object> collec;
            Action<object> adder;

            switch (node.type.collectionType.name)
            {
                case "list":
                    collec = new List<object>();
                    adder = obj => { (collec as List<object>)?.Add(obj); };
                    break;
                case "set":
                    collec = new HashSet<object>();
                    adder = obj => { (collec as HashSet<object>)?.Add(obj); };
                    break;
                case "stack":
                    collec = new Stack<object>();
                    adder = obj => { (collec as Stack<object>)?.Push(obj); };
                    break;
                case "queue":
                    collec = new Queue<object>();
                    adder = obj => { (collec as Queue<object>)?.Enqueue(obj); };
                    break;
                default:
                    throw new Exception(node.type.collectionType.name + " is not a collection type in magia!");
            }

            foreach (var n in node.Expressions)
            {
                n.Accept(this);
                adder(currentValue.o);
            }
            currentValue = new Value(collec);
        }

        

        public override void Visit(BinExprNode node)
        {
            node.Left.Accept(this);
            var v1 = currentValue;
            node.Right.Accept(this);
            var v2 = currentValue;

            switch (node.OperatorType)
            {
                case Operators.Eq when v2.o == v1.o:
                    currentValue = new Value(true);
                    break;
                case Operators.Eq:
                    currentValue = new Value(false);
                    break;
                case Operators.Plus:
                    switch (v1.type.name)
                    {
                        case "text":
                            currentValue = new Value((v1.o as string) + (v2.o as string));
                            break;
                        case "number":
                            currentValue = new Value((double)v1.o + (double)v2.o);
                            break;
                        default:
                            throw new Exception("The Plus operator has not been implemented with operand type: " + v1.GetType().Name);
                    }
                    break;
                case Operators.Bimin:
                    currentValue = new Value((double)v1.o - (double)v2.o);
                    break;
                case Operators.Greater:
                    currentValue = new Value(((double)v1.o > (double)v2.o));
                    break;
                default: throw new Exception(Operators.GetCodeFromInt(node.OperatorType) + " has not been implemented!");
            }
        }
            

        public override void Visit(UnaExprNode node)
        {
            node.Expr.Accept(this);
            if (node.OperatorType == Operators.Umin)
                currentValue = new Value(-((double)currentValue.o));
            else if (node.OperatorType == Operators.Not)
                currentValue = new Value(!((bool)currentValue.o));
        }

        public override void Visit(EdgeCreateNode node)
        {
            CreateNewEdge(node);
        }

        public override void Visit(FuncDeclNode node)
        {
            _currentScope = _callStack.Peek();

            node.Body.Accept(this);

            if (_executionInterrupted == ExeContinue)
                throw new Exception("Continue called outside loop structure.");
            else if (_executionInterrupted == ExeBreak)
                throw new Exception("Break called outside loop structure.");

            _executionInterrupted = null;
        }

        public override void Visit(VarDeclNode node)
        {
            //We find the initial value for the var.
            Value value;

            if (node.DefaultValue is null) {
                value = new Value(PreDefined.GetDefaultValueOfAttributeType(node.type));
            } else {
                node.DefaultValue.Accept(this);
                value = currentValue;
            }

            _currentScope.CreateVar(node.SymbolObject.Name, value);
        }

        public override void Visit(VertexDeclNode node)
        {
            CreateNewVertex(node);
            if (node.Attributes.Statements.Count == 0) return;
            _currentScope.TryGetValue(node.SymbolObject.Name, out var value);
            var v = (Vertex)value.o;
            foreach (var n in node.Attributes.Statements) {
                var a = (AssignNode)n;
                a.Value.Accept(this);
                v.UpdateAttribute(a.Target.Ident, currentValue);
            }
        }

        public override void Visit(AssignNode node)
        {
            node.Value.Accept(this);

            //If there is a source the value should not actually be assigned to this variable but rather as an attribute of the source.
            if (node.Target.Source == null)
            {
                _currentScope.UpdateVar(node.Target.Ident, currentValue);
            }
            else
            {
                var o = DecodeReference(node.Target.Source); // Yes, we need the source, in order to change the target inside the source
                switch (o)
                {
                    case Vertex vertex:
                        vertex.UpdateAttribute(node.Target.Ident, currentValue);
                        break;
                    case Edge edge:
                        edge.UpdateAttribute(node.Target.Ident, currentValue);
                        break;
                    default:
                        throw new Exception($"Tried to access attribute {node.Target.Ident} in type that is not vertex or edge.");
                }
            }
        }

        private object DecodeReference(IdentNode currentSource)
        {
            if (currentSource.Source == null)
            {
                _currentScope.TryGetValue(currentSource.Ident, out var v);
                return v.o;
            }
            
            var vtxOrEdge = DecodeReference(currentSource.Source);

            switch (vtxOrEdge)
            {
                case Vertex vertex:
                    return vertex.attributes[currentSource.Ident];
                case Edge edge:
                    return edge.attributes[currentSource.Ident];
            }
            

            throw new Exception($"Tried to access attribute {currentSource.Ident} in type that is not vertex or edge.");
        }

        public override void Visit(BlockNode node)
        {
            foreach (var s in node.Statements)
            {
                s.Accept(this);
                if (_executionInterrupted != null)
                    return;
            }
        }

        public override void Visit(ForeachNode node)
        {
            var iterationVar = node.IterationVar.SymbolObject.Name;
            _currentScope.CreateVar(iterationVar, null);
            node.Iterator.Accept(this);

            IEnumerable<object> l;

            switch (currentValue.type.collectionType.name)
            {
                case "list":
                    l = (List<object>)currentValue.o;
                    break;
                case "set":
                    l = (HashSet<object>)currentValue.o;
                    break;
                case "stack":
                    l = (Stack<object>)currentValue.o;
                    break;
                case "queue":
                    l = (Queue<object>)currentValue.o;
                    break;
                default:
                    throw new Exception("Cannot iterate over collection type: " + currentValue.type.collectionType.name);
            }

            foreach (var i in l)
            {
                _currentScope.UpdateVar(iterationVar, new Value(i));
                node.Body.Accept(this);

                if (InterruptHandler())
                    break;
            }
        }

        public override void Visit(ForNode node)
        {
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            while ((bool)currentValue.o) {

                node.Body.Accept(this);

                if (InterruptHandler())
                    break;

                node.Iterator.Accept(this);
                node.Condition.Accept(this);
            }
        }



        public override void Visit(IfNode node)
        {
            //If the Condition is null this is an 'else' node that must be execute.
            if (node.Condition == null) {
                node.Body.Accept(this);
                return;
            }

            node.Condition.Accept(this);
            if (currentValue.o.GetType() != typeof(bool)) return;
            var b = (bool)currentValue.o;
            if (b)
                node.Body.Accept(this);
            else
                node.ElseNode?.Accept(this);
        }

        public override void Visit(LoneCallNode node)
        {
            currentValue = null;
            node.Call.Accept(this);
            currentValue = null;
        }



        public override void Visit(WhileNode node)
        {
            //We accept the Condition once to see if we enter the while loop
            node.Condition.Accept(this);
            while ((bool)currentValue.o) {
                //We do the body of the while
                node.Body.Accept(this);

                //The Interrupt handler is given a chance to interrupt.
                if (InterruptHandler())
                    break;

                //We evaluate the condition again just before the while loop does
                node.Condition.Accept(this);
            }
        }

        /// <summary>
        /// Will handle a executionInterrupt.
        /// Will Reset executionInterrupted variable if needed.
        /// </summary>
        /// <returns>Returns true if the iterative control structure should break, false otherwise.</returns>
        private bool InterruptHandler()
        {
            switch (_executionInterrupted)
            {
                //If Continue flag was set it is reset for next round.
                //If break was set we reset it but also break out of the loop
                //If return we break and dont reset as other nodes might want to know that were breaking
                case ExeContinue:
                    _executionInterrupted = null;
                    break;
                case ExeBreak:
                    _executionInterrupted = null;
                    return true;
                case ExeReturn:
                    return true;
                default: throw new Exception(_executionInterrupted + " is not a valid interrupt msg.");
            }

            return false;
        }

        public override void Visit(ReturnNode node)
        {
            node.Ret.Accept(this);
            _executionInterrupted = ExeReturn;
        }

        public override void Visit(BreakNode node)
        {
            _executionInterrupted = ExeBreak;
        }

        public override void Visit(ContinueNode node)
        {
            _executionInterrupted = ExeContinue;
        }

        public override void Visit(NoneConst node)
        {
            currentValue = new Value(new NoneConst());
        }

        public override void Visit(NumConst node)
        {
            currentValue = new Value(node.Value);
        }

        public override void Visit(TextConst node)
        {
            currentValue = new Value(node.Value);
        }
        public override void Visit(BoolConst node)
        {
            currentValue = new Value(node.Value);
        }

        //The Magia node executes everything in it that is not a funcDeclNode and not a HeadNode.
        public override void Visit(Magia node)
        {
            MoveFunctions(node);
            foreach(var n in node.block.Statements)
            {
                if (n.GetType() != typeof(HeadNode) && n.GetType() != typeof(FuncDeclNode))
                {
                    n.Accept(this);
                }
            }
        }
        
        ////Below Functions aren't supposed to be implemented as they contain no runnable code!////
        public override void Visit(HeadNode node)
        {
            throw new Exception("HeadNode should not be visited during execution!");
        }
    }
}
