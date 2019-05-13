using System;
using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
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
        Magia ast;


        //The Defnition of a vertex and edge attributions.
        Dictionary<string, BaseType> vertexAttr = new Dictionary<string, BaseType>();
        Dictionary<string, BaseType> edgeAttr = new Dictionary<string, BaseType>();

        //The Scene will be printet when a print statement is encountered.
        List<vertex> scene = new List<vertex>();

        //The functions
        Dictionary<string, FuncDeclNode> functions = new Dictionary<string, FuncDeclNode>();
        List<Node> main = new List<Node>();

        public override string AppropriateFileName { get; } = "CompileRes.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        public Executor(Parser parser)
        {
            Table = parser.tab;
            ast = parser.mainNode;
            moveAttrDefinitions();
            moveFunctions();
            moveMain();
            start();
        }

        private void moveMain()
        {
            ast.block.Statements.ForEach(s => {
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

        private void moveFunctions()
        {
            foreach (StmtNode s in ast.block.Statements) {
                if (s.GetType() == typeof(FuncDeclNode))
                {
                    FuncDeclNode f = (FuncDeclNode)s;
                    functions.Add(f.SymbolObject.Name, f);
                }
            }
        }
        private void CreateNewVertex(VertexDeclNode vertex)
        {
            scene.Add(new vertex(vertex, vertexAttr));
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
                vertexAttr.Add(v.Key, v.Value.type);
            }
            foreach (KeyValuePair<string, Obj> v in Table.edgeAttr.GetDic())
            {
                edgeAttr.Add(v.Key, v.Value.type);
            }
        }

        public override void Visit(CallNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(VarNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(MultiDecl multiDecl)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BoolConst node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(CollecConst node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(NoneConst node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(NumConst node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(TextConst node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BinExprNode node)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void Visit(VarDeclNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(VertexDeclNode node)
        {
            CreateNewVertex(node);
        }

        public override void Visit(AssignNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BlockNode node)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void Visit(LoneCallNode node)
        {
            functions.TryGetValue(node.Call.Ident, out FuncDeclNode f);
        }

        public override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
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
