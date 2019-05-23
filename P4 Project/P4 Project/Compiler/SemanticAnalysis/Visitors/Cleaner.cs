using System.Collections.Generic;
using System.Text;
using P4_Project.AST;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.SymbolTable;
using System.Linq;

namespace P4_Project.Compiler.SemanticAnalysis.Visitors
{
    public sealed class Cleaner : Visitor
    {
        //This Visitor checks for obviously missing/wrong things.
        //1. All function calls corresponds to a function. 
        //2. Calls have the correct amount of parameters when calling 
        //3. Expressions aren't outright missing or null 
        //4. functions with a non "none" return type must have at least one return inside them! 
        //5. Checks that at maximum one of each type header exists!
        //6. Remove the function "SymbolObject" from the list of symbols, but register type info in the function's scope.
        public override string AppropriateFileName { get; } = "Clean.txt";
        public override StringBuilder Result { get; } = new StringBuilder();
        public override List<string> ErrorList { get; } = new List<string>();
        private SymTable Table { get; }

        private bool _vertexHeadExists;
        private bool _edgeHeadExists;

        private int _directionType = 0;


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
            foreach (var baseTypes in Table.FindParameterListOfFunction(node.Ident))
            {
                if (node.Parameters.Expressions.Count != baseTypes.Count) continue;
                return;
            }
            
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

            //The function checks if the direction of the graph has been set, sets it if not,
            //and if the node will create edges of the same type as the graph.
            if (_directionType == 0)
                if (node.Operator == 17)
                    _directionType = 1;
                else
                    _directionType = 2;
            else if (node.Operator != 17 && _directionType == 1)
                ErrorList.Add("The direction type of the program is " + Operators.GetCodeFromInt(_directionType) + ", but the direction type of the edge from " + node.LeftSide.Ident + " to " + node.RightSide[0].Item1.Ident +  " and more is " + node.GetCodeOfOperator());
            else if(node.Operator == 17 && _directionType == 2)
                ErrorList.Add("The direction type of the program is " + Operators.GetCodeFromInt(_directionType) + ", but the direction type of the edge from " + node.LeftSide.Ident + " to " + node.RightSide[0].Item1.Ident + " and more is " + node.GetCodeOfOperator());



            //3. Expressions aren't outright missing or null 
            if (node.RightSide.Count == 0)
                ErrorList.Add("The right side of an edge creation exists, but have no expressions inside: " + node.GetCodeOfOperator());
            //
        }

        public override void Visit(FuncDeclNode node)
        {
            node.Parameters.Accept(this);
            node.Body.Accept(this);

            //4. functions with a non "none" return type must have at least one return inside them!
            if (node.SymbolObject.Type.returnType.name == "none") return;
            foreach (var n in node.Body.Statements)
                if (n.GetType() == typeof(ReturnNode))
                    return;
            
            ErrorList.Add($"Function '{node.SymbolObject.Name}' has no return statement in its body and is not declared to return none!");
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

            //5. Checks that at maximum one of each type header exists.
            switch (node.type.name)
            {
                case "edge" when !_edgeHeadExists:
                    _edgeHeadExists = true;
                    return;
                case "edge" when _edgeHeadExists:
                    ErrorList.Add("Only one edge-header is allowed!");
                    return;
                case "vertex" when !_vertexHeadExists:
                    _vertexHeadExists = true;
                    return;
                case "vertex" when _vertexHeadExists:
                    ErrorList.Add("Only one vertex-header is allowed!");
                    return;
                default: ErrorList.Add("HeadNode must be type edge or vertex");
                    return;
            }
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
            
            //6. Remove the function "SymbolObject" from the list of symbols, but register type info in the function's scope.
            foreach(FuncDeclNode fdecl in node.block.Statements.Where(stmt => stmt is FuncDeclNode).Cast<FuncDeclNode>())
            {
                // Save the type in the scope that corresponds to the function.
                Table.GetInnerScopes().First(s => s.name == fdecl.SymbolObject.Name).type = fdecl.SymbolObject.Type;
                // Remove function symbol  
                Table.RemoveVar(fdecl.SymbolObject);
            }
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
