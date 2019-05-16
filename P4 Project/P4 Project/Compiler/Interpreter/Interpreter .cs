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
    public class Executor : Visitor
    {
        //This is the given information.
        //That will be used to execute the entire program.
        public virtual SymTable Table { get; set; }

        //The Defnition of a vertex and edge attributions.
        Dictionary<string, BaseType> defVertexAttr = new Dictionary<string, BaseType>();
        Dictionary<string, BaseType> defEdgeAttr = new Dictionary<string, BaseType>();

        //The Scene will be printet when a print statement is encountered.
        internal List<Vertex> scene = new List<Vertex>();

        //The functions
        Dictionary<string, FuncDeclNode> functions = new Dictionary<string, FuncDeclNode>();

        //All the main nodes
        List<Node> main = new List<Node>();
        Scope mainScope = new Scope(null);

        Stack<Scope> callStack = new Stack<Scope>();

        string executionInterrupted = null;
        const string exeBreak = "break", exeContinue = "continue", exeReturn = "return";

        //Current scope used to keep track of the current scope at runtime
        internal Scope currentscope;
        internal Value currentValue;

        public override string AppropriateFileName { get; } = "CompileRes.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();

        public Executor(Parser parser)
        {
            Table = parser.tab;
            currentscope = mainScope;
            moveAttrDefinitions();
            moveFunctions(parser.mainNode);
            moveMain(parser.mainNode);
            start();
        }

        private void moveMain(Magia node)
        {
            node.block.Statements.ForEach(s => {
                if (s.GetType() != typeof(FuncDeclNode) && s.GetType() != typeof(HeadNode))
                    main.Add(s);
            });
        }

        private void start()
        {
            //The main is executet
            foreach(Node n in main)
            {
                n.Accept(this);
            }
        }

        private void moveFunctions(Magia node)
        {
            foreach (StmtNode s in node.block.Statements) {
                if (s.GetType() == typeof(FuncDeclNode))
                {
                    FuncDeclNode f = (FuncDeclNode)s;
                    functions.Add(f.SymbolObject.Name, f);
                }
            }
        }
        private void CreateNewVertex(VertexDeclNode vertex)
        {
            //The vertex is created
            Dictionary<string, Value> values = new Dictionary<string, Value>();
            foreach (KeyValuePair<string, BaseType> vattr in defVertexAttr) 
                values.Add(vattr.Key, new Value(PreDefined.GetDefaultValueOfAttributeType(vattr.Value)));
            Vertex v = new Vertex(vertex, values);
            //We add it to the scene and the currentscope
            scene.Add(v);
            currentscope.CreateVar(v.identifier,new Value(v));
        }
        private void CreateNewEdge(EdgeCreateNode edge)
        {
            currentscope.TryGetValue(edge.LeftSide.Ident, out Value v);
            Vertex from = (Vertex)v.o;
            foreach (Tuple<IdentNode, List<AssignNode>> tuple in edge.RightSide) {
                Dictionary<string, Value> values = new Dictionary<string, Value>();
                foreach (KeyValuePair<string, BaseType> eattr in defEdgeAttr)
                    values.Add(eattr.Key, new Value(PreDefined.GetDefaultValueOfAttributeType(eattr.Value)));
                currentscope.TryGetValue(tuple.Item1.Ident, out Value v1);
                Vertex to = (Vertex)v1.o;
                Edge e = new Edge(from, edge.Operator, to, values);
                foreach (AssignNode l in tuple.Item2)
                {
                    l.Value.Accept(this);
                    e.UpdateAttribute(l.Target.Ident, currentValue);
                }
                from.edges.Add(e);
            }
        }

        /// <summary>
        /// Function runs only once in the constructor of executor.
        /// Its responseability is to move attributes to into the executor from the Table.
        /// </summary>
        private void moveAttrDefinitions()
        {
            foreach (KeyValuePair<string, Obj> v in Table.vertexAttr.GetDic())
            {
                defVertexAttr.Add(v.Key, v.Value.Type);
            }
            foreach (KeyValuePair<string, Obj> v in Table.edgeAttr.GetDic())
            {
                defEdgeAttr.Add(v.Key, v.Value.Type);
            }
        }

        public override void Visit(CallNode node)
        {
            //Lookup the function
            functions.TryGetValue(node.Ident, out FuncDeclNode f);

            //If the function was not found we check if its a predefined function
            if (f == null && PreDefined.PreDefinedFunctions.Contains(node.Ident))
            {
                //We collect the parameters by visting each expression and adding that resulting current value to value list
                List<Value> parameters = new List<Value>();
                node.Parameters.Expressions.ForEach(e => {
                    e.Accept(this);
                    parameters.Add(currentValue);
                });
                PreDefined.DoPreDefFunction(node.Ident, this, parameters);
                return;
            }
            else if (f == null)
                throw new Exception(node.Ident + " is not a neither a predefined function or a userdefined function");


            //We create a new callScope on the call stack
            callStack.Push(new Scope(currentscope));

            //We add the parameters as variables in that scope using the retrieved function
            for (int i = f.Parameters.Statements.Count; i > 0; i--)
            {
                VarDeclNode v = (VarDeclNode) f.Parameters.Statements[i-1];
                node.Parameters.Expressions[i-1].Accept(this);
                callStack.Peek().CreateVar(v.SymbolObject.Name, currentValue);
            }
            //We Visit that function to execute it
            f.Accept(this);

            callStack.Pop();
            if (callStack.Count == 0)
                currentscope = mainScope;
            else currentscope = callStack.Peek();
        }

        public override void Visit(VarNode node)
        {
            if (node.Source == null)
            {
                currentscope.TryGetValue(node.Ident, out Value value);
                currentValue = value;
            }
            else
            {
                object o = DecodeReference(node.Source); // Yes, we need the source, in order to change the attribute inside the source
                if (o is Vertex)
                {
                    Vertex v = (Vertex)o;
                    currentValue = v.attributes[node.Ident];
                }
                else if (o is Edge)
                {
                    Edge e = (Edge)o;
                    currentValue = e.attributes[node.Ident];
                }
                else
                    throw new Exception($"Tried to access attribute {node.Ident} in type that is not vertex or edge.");
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

            if (node.type.collectionType.name == "list")
            {
                collec = new List<object>();
                adder = obj => { (collec as List<object>).Add(obj); };
            }
            else if (node.type.collectionType.name == "set")
            {
                collec = new HashSet<object>();
                adder = obj => { (collec as HashSet<object>).Add(obj); };
            }
            else if (node.type.collectionType.name == "stack")
            {
                collec = new Stack<object>();
                adder = obj => { (collec as Stack<object>).Push(obj); };
            }
            else if (node.type.collectionType.name == "queue")
            {
                collec = new Queue<object>();
                adder = obj => { (collec as Queue<object>).Enqueue(obj); };
            }
            else
                throw new Exception(node.type.collectionType.name + " is not a collection type in magia!");

            foreach (Node n in node.Expressions)
            {
                n.Accept(this);
                adder(currentValue.o);
            }
            currentValue = new Value(collec);
        }

        

        public override void Visit(BinExprNode node)
        {
            node.Left.Accept(this);
            Value v1 = currentValue;
            node.Right.Accept(this);
            Value v2 = currentValue;

            if (node.OperatorType == Operators.Eq)
            {
                if (v2.o.ToString() == v1.o.ToString())
                    currentValue = new Value(true);
                else currentValue = new Value(false);
            }
            else if (node.OperatorType == Operators.Plus)
            {
                if (v1.type.name == "text")
                {
                    currentValue = new Value((v1.o as string) + (v2.o as string));
                }
                else if (v1.type.name == "number")
                {
                    currentValue = new Value((double)v1.o + (double)v2.o);
                }
                else throw new Exception("The Plus operator has not been implemented with operand type: " + v1.GetType().Name);
            }
            else if (node.OperatorType == Operators.Bimin)
            {
                currentValue = new Value((double)v1.o - (double)v2.o);
            }
            else if (node.OperatorType == Operators.Greater)
            {
                currentValue = new Value(((double)v1.o > (double)v2.o));
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
            currentscope = callStack.Peek();

            node.Body.Accept(this);

            if (executionInterrupted == exeContinue)
                throw new Exception("Continue called outside loop structure.");
            else if (executionInterrupted == exeBreak)
                throw new Exception("Break called outside loop structure.");

            executionInterrupted = null;
        }

        public override void Visit(VarDeclNode node)
        {
            //We find the inital value for the var
            Value value;

            if (node.DefaultValue is null) {
                value = new Value(PreDefined.GetDefaultValueOfAttributeType(node.type));
            } else {
                node.DefaultValue.Accept(this);
                value = currentValue;
            }

            currentscope.CreateVar(node.SymbolObject.Name, value);
        }

        public override void Visit(VertexDeclNode node)
        {
            CreateNewVertex(node);
            if (node.Attributes.Statements.Count != 0) {
                currentscope.TryGetValue(node.SymbolObject.Name, out Value value);
                Vertex v = (Vertex)value.o;
                foreach (Node n in node.Attributes.Statements) {
                    AssignNode a = (AssignNode)n;
                    a.Value.Accept(this);
                    v.UpdateAttribute(a.Target.Ident, currentValue);
                }
            }
        }

        public override void Visit(AssignNode node)
        {
            node.Value.Accept(this);

            //If there is a source the value should not actually be assigned to this variable but rather as an attribute of the source.
            if (node.Target.Source == null)
            {
                currentscope.UpdateVar(node.Target.Ident, currentValue);
            }
            else
            {
                object o = DecodeReference(node.Target.Source); // Yes, we need the source, in order to change the target inside the source
                if (o is Vertex)
                    (o as Vertex).UpdateAttribute(node.Target.Ident, currentValue);
                else if (o is Edge)
                    (o as Edge).UpdateAttribute(node.Target.Ident, currentValue);
                else
                    throw new Exception($"Tried to access attribute {node.Target.Ident} in type that is not vertex or edge.");
            }
        }

        private object DecodeReference(IdentNode currentSource)
        {
            if (currentSource.Source == null)
            {
                currentscope.TryGetValue(currentSource.Ident, out Value v);
                return v.o;
            }
            else
            {
                object vtxOrEdge = DecodeReference(currentSource.Source);

                if (vtxOrEdge is Vertex)
                    return (vtxOrEdge as Vertex).attributes[currentSource.Ident];
                if (vtxOrEdge is Edge)
                    return (vtxOrEdge as Edge).attributes[currentSource.Ident];
            }

            throw new Exception($"Tried to access attribute {currentSource.Ident} in type that is not vertex or edge.");
        }

        public override void Visit(BlockNode node)
        {
            foreach (var s in node.Statements)
            {
                s.Accept(this);
                if (executionInterrupted != null)
                    return;
            }
        }

        public override void Visit(ForeachNode node)
        {
            string iterationVar = node.IterationVar.SymbolObject.Name;
            currentscope.CreateVar(iterationVar, null);
            node.Iterator.Accept(this);

            IEnumerable<object> l = null;

            if (currentValue.type.collectionType.name == "list")
                l = (List<object>)currentValue.o;
            else if (currentValue.type.collectionType.name == "set")
                l = (HashSet<object>)currentValue.o;
            else if (currentValue.type.collectionType.name == "stack")
                l = (Stack<object>)currentValue.o;
            else if (currentValue.type.collectionType.name == "queue")
                l = (Queue<object>)currentValue.o;
            else
                throw new Exception("Cannot iterate over collection type: " + currentValue.type.collectionType.name);

            foreach (var i in l)
            {
                currentscope.UpdateVar(iterationVar, new Value(i));
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
            //If the Condition is null this is an 'else' node that must be executet.
            if (node.Condition == null) {
                node.Body.Accept(this);
                return;
            }

            node.Condition.Accept(this);
            if (currentValue.o.GetType() == typeof(bool))
            {
                bool b = (bool)currentValue.o;
                if (b)
                    node.Body.Accept(this);
                else
                    node.ElseNode?.Accept(this);
            }
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

                //The Interupt handler is given a chance to interrupt.
                if (InterruptHandler())
                    break;

                //We evaulate the condition again just before the while loop does
                node.Condition.Accept(this);
            }
        }

        /// <summary>
        /// Will handle a executionInterupt.
        /// Will Reset executionInterrupted variable if needed.
        /// </summary>
        /// <param name="executionInterrupted"></param>
        /// <returns>Returns true if the iterative control structure should break, false otherwise.</returns>
        private bool InterruptHandler()
        {
            //If Continue flag was set it is reset for next round.
            //If break was set we reset it but also break out of the loop
            //If return we break and dont reset as other nodes might want to know that were breaking
            if (executionInterrupted == exeContinue)
                executionInterrupted = null;
            else if (executionInterrupted == exeBreak){
                executionInterrupted = null;
                return true;
            }else if (executionInterrupted == exeReturn)
                return true;
            return false;
        }

        public override void Visit(ReturnNode node)
        {
            node.Ret.Accept(this);
            executionInterrupted = exeReturn;
        }

        public override void Visit(BreakNode node)
        {
            executionInterrupted = exeBreak;
        }

        public override void Visit(ContinueNode node)
        {
            executionInterrupted = exeContinue;
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

        ////Below Functions arent supposed to be implimented as they contain no runable code!////
        public override void Visit(HeadNode node)
        {
            throw new NotImplementedException();
        }
        public override void Visit(Magia node)
        {
            throw new NotImplementedException();
        }
    }
}
