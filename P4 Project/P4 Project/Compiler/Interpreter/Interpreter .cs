using System;
using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.Interpreter;
using P4_Project.Compiler.Interpreter.Types;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.SymbolTable;
using P4_Project.Visitors;

namespace P4_Project.Compiler.Executor
{
    public class Executor : Visitor
    {
        //This is the given information.
        //That will be used to execute the entire program.
        public override SymTable Table { get; set; }

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
            foreach (KeyValuePair<string, BaseType> vattr in defVertexAttr) {
                values.Add(vattr.Key, new Value(PreDefined.GetDefualtValueOfAttributeType(vattr.Value)));
            }
            Vertex v = new Vertex(vertex, values);
            //We add it to the scene and the currentscope
            scene.Add(v);
            currentscope.Values.Add(v.identifyer,new Value(v));
        }
        private void CreateNewEdge(List<Tuple<string, string>> attributesAndValues)
        {
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
            //We create a new callScope on the call stack
            callStack.Push(new Scope(currentscope));
            currentscope = callStack.Peek();

            //Lookup the function
            functions.TryGetValue(node.Ident, out FuncDeclNode f);

            //If the function was not found we check if its a predefined function
            if (f == null && PreDefined.preDefinedFunctions.Contains(f.SymbolObject.Name))
            {
                //We collect the parameters by visting each expression and adding that resulting current value to value list
                List<Value> parameters = new List<Value>();
                node.Parameters.Expressions.ForEach(e => {
                    e.Accept(this);
                    parameters.Add(currentValue);
                });
                PreDefined.DoPreDefFunction(f.SymbolObject.Name, this, parameters);
                return;
            }
            else if (f == null) throw new Exception(node.Ident + " is not a neither a predefined function or a userdefined function");

            //We add the parameters as variables in that scope using the retrieved function
            for (int i = f.Parameters.Statements.Count; i > 0; i--)
            {
                VarDeclNode v = (VarDeclNode) f.Parameters.Statements[i-1];
                node.Parameters.Expressions[i-1].Accept(this);
                currentscope.Values.Add(v.SymbolObject.Name, currentValue);
            }
            //We Visit that function to execute it
            f.Accept(this);
            
        }

        public override void Visit(VarNode node)
        {
            currentscope.Values.TryGetValue(node.Ident, out Value value);
            currentValue = value;
        }

        public override void Visit(MultiDecl multiDecl)
        {
            multiDecl.Decls.ForEach(d => d.Accept(this));
        }

        public override void Visit(BoolConst node)
        {
            currentValue = new Value(node.Value);
        }

        public override void Visit(CollecConst node)
        {
            if (node.type.collectionType.name == "list")
            {
                List<object> list = new List<object>();
                foreach (Node n in node.Expressions)
                {
                    n.Accept(this);
                    list.Add(currentValue.o);
                }
                currentValue = new Value(list);
            }
            else
            if (node.type.collectionType.name == "set")
            {
                HashSet<object> set = new HashSet<object>();
                foreach (Node n in node.Expressions)
                {
                    n.Accept(this);
                    set.Add(currentValue.o);
                }
                currentValue = new Value(set);
            }
            else
            if (node.type.collectionType.name == "stack")
            {
                Stack<object> stack = new Stack<object>();
                foreach (Node n in node.Expressions)
                {
                    n.Accept(this);
                    stack.Push(currentValue.o);
                }
                currentValue = new Value(stack);
            }
            else
            if (node.type.collectionType.name == "queue")
            {
                Queue<object> queue = new Queue<object>();
                foreach (Node n in node.Expressions)
                {
                    n.Accept(this);
                    queue.Enqueue(currentValue.o);
                }
                currentValue = new Value(queue);
            }
            else throw new Exception(node.type.collectionType.name + " is not a collectiontype in magia!");
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

        public override void Visit(BinExprNode node)
        {
            //operator 3 is '=='
            if (node.OperatorType == 3) {
                node.Left.Accept(this);
                Value v1 = currentValue;
                node.Right.Accept(this);
                Value v2 = currentValue;
                if (v2.o.ToString() == v1.o.ToString())
                    currentValue = new Value(true);
                else currentValue = new Value(false);
            }
        }

        public override void Visit(UnaExprNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(EdgeCreateNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FuncDeclNode node)
        {
            node.Body.Accept(this);

        }

        public override void Visit(VarDeclNode node)
        {
            //We find the inital value for the var
            Value value;
            if (node.DefaultValue is null) {
                value = new Value(PreDefined.GetDefualtValueOfAttributeType(node.type));
            } else {
                node.DefaultValue.Accept(this);
                value = currentValue;
            }
            currentscope.Values.Add(node.SymbolObject.Name, value);
        }

        public override void Visit(VertexDeclNode node)
        {
            CreateNewVertex(node);
            if (node.Attributes.Statements.Count != 0) {
                currentscope.Values.TryGetValue(node.SymbolObject.Name, out Value value);
                Vertex v = (Vertex)value.o;
                foreach (Node n in node.Attributes.Statements) {
                    AssignNode a = (AssignNode)n;
                    a.Value.Accept(this);
                    v.updateAttribute(a.Target.Ident, currentValue);
                }
            }
        }

        public override void Visit(AssignNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BlockNode node)
        {
            node.Statements.ForEach(s => s.Accept(this));
        }

        public override void Visit(ForeachNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ForNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(HeadNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IfNode node)
        {
            node.Condition.Accept(this);
            if (currentValue.o == typeof(bool))
            {
                bool b = (bool)currentValue.o;
                if (b)
                    node.Body.Accept(this);
                else node?.ElseNode.Accept(this);
            }
        }

        public override void Visit(LoneCallNode node)
        {
            currentValue = null;
            node.Call.Accept(this);
            currentValue = null; ;
        }

        public override void Visit(ReturnNode node)
        {
            node.Ret.Accept(this);
            callStack.Pop();
            if (callStack.Count == 0) 
                currentscope = mainScope;
            else currentscope = callStack.Peek();
        }

        public override void Visit(WhileNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BreakNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ContinueNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(Magia node)
        {
            throw new NotImplementedException();
        }
    }
}
