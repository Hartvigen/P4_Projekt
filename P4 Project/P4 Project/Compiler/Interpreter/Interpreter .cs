using System;
using System.Collections.Generic;
using P4_Project.AST;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.Compiler.SyntaxAnalysis;
using P4_Project.SymbolTable;

namespace P4_Project.Compiler.Executor
{
    public class Executor
    {
        //This is the given information.
        //That will be used to execute the entire program.
        SymTable Table;
        Magia ast;


        //The Defnition of a vertex and edge attributions.
        Dictionary<string, BaseType> vertexAttr = new Dictionary<string, BaseType>();
        Dictionary<string, BaseType> edgeAttr = new Dictionary<string, BaseType>();

        //The Scene will be printet when a print statement is encountered.
        Tuple<List<vertex>, List<edge>> scene;

        //The functions
        List<FuncDeclNode> functions = new List<FuncDeclNode>();
        List<Node> main = new List<Node>();

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
            throw new NotImplementedException();
        }

        private void moveFunctions()
        {
            ast.block.Statements.ForEach(s => {
                if (s.GetType() == typeof(FuncDeclNode))
                    functions.Add((FuncDeclNode)s);
            });
        }
        private void CreateNewVertex(List<Tuple<string, string>> attributesAndValues)
        {
            vertex v = new vertex(attributesAndValues, vertexAttr);
        }
        private void CreateNewEdge(List<Tuple<string, string>> attributesAndValues)
        {
            vertex v = new vertex(attributesAndValues, edgeAttr);
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
    }
}
