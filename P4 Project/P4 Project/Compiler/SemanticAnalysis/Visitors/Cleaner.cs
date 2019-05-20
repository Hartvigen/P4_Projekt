using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.SymbolTable;

namespace P4_Project.Compiler.SemanticAnalysis.Visitors
{
    public sealed class Cleaner : Visitor
    {
        //This Visitor checks for obviously missing/wrong things. 
        //Like: 
        //1. All function calls corresponds to a function. 
        //2. Calls have the correct amount of parameters when calling 
        //3. Expressions aren't outright missing or null 
        //4. functions with a non "none" return type must have at least one return inside them! 
        //5. Checks that at maximum one of each type header exists! 
        public override string AppropriateFileName { get; } = "Clean.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        private SymTable Table { get; }

        private bool _vertexHeadExists;
        private bool _edgeHeadExists;


        public Cleaner(SymTable table) {
            Table = table;
        }

        public override void Visit(CallNode node)
        {
            node.Parameters.Accept(this);

            //1. All function calls corresponds to a function. 
            if (!Table.FunctionExists(node.Ident))
                ErrorList.Add($"Call made to function '{node.Ident}', which is not a declared function and not a predefined function");

            //2. Calls have the correct amount of parameters when calling
            var valid = false;
            foreach (var baseTypes in Table.FindParameterListOfFunction(node.Ident))
            {
                if (node.Parameters.Expressions.Count == baseTypes.Count)
                {
                    valid = true;
                    break;
                }
            }
            
            if (!valid)
                ErrorList.Add($"The function '{node.Ident}' cannot be called with '{node.Parameters.Expressions.Count}' parameters");
        }
        public override void Visit(VarNode node)
        {
            node.Source?.Accept(this);
        }

        public override void Visit(BoolConst node)
        {
        }

        public override void Visit(CollecConst node)
        {
            node.Expressions.ForEach(n => n.Accept(this));
        }

        public override void Visit(NoneConst node)
        {
        }

        public override void Visit(NumConst node)
        {
        }

        public override void Visit(TextConst node)
        {
        }

        public override void Visit(BinExprNode node)
        {
            //3. Expressions aren't outright missing or null
            if (node.Left is null || node.Right is null)
            {
                ErrorList.Add("BinExprNode has null operands");
            }

            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        public override void Visit(UnaExprNode node)
        {
            node.Expr.Accept(this);
        }

        public override void Visit(EdgeCreateNode node)
        {
            node.LeftSide.Accept(this);
            node.RightSide.ForEach(t => { t.Item1.Accept(this); t.Item2.ForEach(l => l.Accept(this)); });

            //3. Expressions aren't outright missing or null 
            if (node.RightSide.Count == 0)
                ErrorList.Add("The right side of an edge creation exists, but have no expressions inside: " + node.GetCodeOfOperator());
        }

        public override void Visit(FuncDeclNode node)
        {
            node.Parameters.Accept(this);
            node.Body.Accept(this);

            //4. functions with a non "none" return type must have at least one return inside them! 
            if (node.SymbolObject.Type.returnType != "none")
            {
                bool retExists = false;
                node.Body.Statements.ForEach(n =>
                {
                    if (n.GetType() == typeof(ReturnNode))
                        retExists = true;
                });
                if (retExists)
                    return;
                ErrorList.Add($"Function '{node.SymbolObject.Name}' has no return statement in its body and is not declared to return none!");
            }
        }

        public override void Visit(VarDeclNode node)
        {
            node.DefaultValue?.Accept(this);
        }

        public override void Visit(VertexDeclNode node)
        {
            node.Attributes.Accept(this);
        }

        public override void Visit(AssignNode node)
        {
            node.Target.Accept(this);
            node.Value.Accept(this);
        }

        public override void Visit(BlockNode node)
        {
            node.Statements.ForEach(n => n.Accept(this));
        }

        public override void Visit(ForeachNode node)
        {
            node.IterationVar.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
        }

        public override void Visit(ForNode node)
        {
            node.Initializer.Accept(this);
            node.Condition.Accept(this);
            node.Iterator.Accept(this);
            node.Body.Accept(this);
        }

        public override void Visit(HeadNode node)
        {
            node.attrDeclBlock.Accept(this);

            //5. Checks that at maximum one of each type header exists! 
            if (node.type.name == "edge" && !_edgeHeadExists)
            {
                _edgeHeadExists = true;
                return;
            }

            if (node.type.name == "vertex" && !_vertexHeadExists)
            {
                _vertexHeadExists = true;
                return;
            }

            if (_edgeHeadExists && node.type.name == "edge")
                ErrorList.Add("Only one edge-header is allowed!");
            else if(_vertexHeadExists && node.type.name == "vertex")
                ErrorList.Add("Only one vertex-header is allowed!");
        }

        public override void Visit(IfNode node)
        {
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.ElseNode?.Accept(this);
        }

        public override void Visit(LoneCallNode node)
        {
            node.Call.Accept(this);
        }

        public override void Visit(ReturnNode node)
        {
            node.Ret.Accept(this);
        }

        public override void Visit(WhileNode node)
        {
            node.Condition.Accept(this);
            node.Body.Accept(this);
        }

        public override void Visit(Magia node)
        {
            node.block.Accept(this);            
        }

        public override void Visit(BreakNode node)
        {
        }

        public override void Visit(ContinueNode node)
        {
        }

        public override void Visit(MultiDecl node)
        {
            node.Decls.ForEach(n => n.Accept(this));
        }
    }
}
