using System;
using P4_Project.AST;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using static P4_Project.SymbolTable.SymTable;
using System.Collections.Generic;
using P4_Project.SymbolTable;

namespace P4_Project.Compiler.SyntaxAnalysis
{



    public class Parser
    {
        public const int _EOF = 0;
        public const int _IDENT = 1;
        public const int _NUMBER = 2;
        public const int _TEXT = 3;
        public const int maxT = 53;

        const bool _T = true;
        const bool _x = false;
        const int minErrDist = 2;

        public Scanner scanner;
        public Errors errors;

        public Token t;    // last recognized token
        public Token la;   // lookahead token
        int errDist = minErrDist;

        public Magia mainNode;
        public SymTable tab;



        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            errors = new Errors();
            tab = new SymTable(null, this);
        }

        void SynErr(int n)
        {
            if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
            errDist = 0;
        }

        public void SemErr(string msg)
        {
            if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
            errDist = 0;
        }

        void Get()
        {
            for (; ; )
            {
                t = la;
                la = scanner.Scan();
                if (la.kind <= maxT) { ++errDist; break; }

                la = t;
            }
        }

        void Expect(int n)
        {
            if (la.kind == n) Get(); else { SynErr(n); }
        }

        bool StartOf(int s)
        {
            return set[s, la.kind];
        }

        void ExpectWeak(int n, int follow)
        {
            if (la.kind == n) Get();
            else
            {
                SynErr(n);
                while (!StartOf(follow)) Get();
            }
        }


        bool WeakSeparator(int n, int syFol, int repFol)
        {
            int kind = la.kind;
            if (kind == n) { Get(); return true; }
            else if (StartOf(repFol)) { return false; }
            else
            {
                SynErr(n);
                while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind]))
                {
                    Get();
                    kind = la.kind;
                }
                return StartOf(syFol);
            }
        }


        void MAGIA()
        {
            mainNode = null; BlockNode mainBlock = new BlockNode();
            while (la.kind == 4)
            {
                while (!(la.kind == 0 || la.kind == 4)) { SynErr(54); Get(); }
                Head(out HeadNode head);
                mainBlock.Add(head);
            }
            while (StartOf(1))
            {
                while (!(StartOf(2))) { SynErr(55); Get(); }
                Stmt(out StmtNode stmt);
                mainBlock.Add(stmt);
            }
            while (la.kind == 11)
            {
                while (!(la.kind == 0 || la.kind == 11)) { SynErr(56); Get(); }
                FuncDecl(out FuncDeclNode funcDecl);
                mainBlock.Add(funcDecl);
            }
            mainNode = new Magia(mainBlock);
        }

        void Head(out HeadNode head)
        {
            head = null; VarDeclNode attrDecl = null;
            Expect(4);
            if (la.kind == 5)
            {
                Get();
                head = new HeadNode(HeadNode.Vertex);
            }
            else if (la.kind == 6)
            {
                Get();
                head = new HeadNode(HeadNode.Edge);
            }
            else SynErr(57);
            ExpectWeak(7, 3);
            VarDecl(out attrDecl);
            head?.AddAttr(attrDecl);
            while (WeakSeparator(8, 4, 5))
            {
                VarDecl(out attrDecl);
                head?.AddAttr(attrDecl);
            }
            ExpectWeak(9, 6);
            Expect(10);
        }

        void Stmt(out StmtNode stmt)
        {
            stmt = null;
            if (StartOf(7))
            {
                StructStmt(out stmt);
            }
            else if (la.kind == 1)
            {
                SimpleStmt(out stmt);
            }
            else if (la.kind == 26 || la.kind == 27 || la.kind == 28)
            {
                KeywordStmt(out stmt);
            }
            else if (StartOf(4))
            {
                FullDecl(out stmt);
            }
            else SynErr(58);
        }

        void FuncDecl(out FuncDeclNode funcNode)
        {
            funcNode = null; string funcName = ""; BaseType returnType = null; BaseType protocol = null;
            Expect(11);
            if (StartOf(4))
            {
                Type(out returnType);
            }
            else if (la.kind == 12)
            {
                Get();
            }
            else SynErr(59);
            Expect(1);
            funcName = t.val; BlockNode paramBlock = new BlockNode(); BlockNode stmtBlock = new BlockNode();
            ExpectWeak(7, 8);
            tab = tab.OpenScope(funcName); VarDeclNode paramDecl = null; List<BaseType> parameterTypes = new List<BaseType>();
            if (la.val != ")" && la.val != "{")
            {
                VarDecl(out paramDecl);
                paramBlock.Add(paramDecl); parameterTypes.Add(paramDecl.SymbolObject.type);
                while (WeakSeparator(8, 4, 5))
                {
                    VarDecl(out paramDecl);
                    paramBlock.Add(paramDecl); parameterTypes.Add(paramDecl.SymbolObject.type);
                }
            }
            protocol = new BaseType(returnType, parameterTypes);
            ExpectWeak(9, 9);
            ExpectWeak(13, 10);
            StmtNode stmt = null;
            while (StartOf(1))
            {
                while (!(StartOf(2))) { SynErr(60); Get(); }
                Stmt(out stmt);
                stmtBlock.Add(stmt);
            }
            ExpectWeak(14, 3);
            SymTable funcScope = tab; tab = tab.CloseScope();
            Obj funcObj = tab.NewObj(funcName, protocol, Func, funcScope);
            funcNode = new FuncDeclNode(funcObj, paramBlock, stmtBlock);

        }

        void VarDecl(out VarDeclNode varDecl)
        {
            varDecl = null; BaseType type = null; string name = null; ExprNode value = null;
            Type(out type);
            Expect(1);
            name = t.val;
            if (la.kind == 29)
            {
                Assign(out value);
            }
            varDecl = new VarDeclNode(tab.NewObj(name, type, Var), value);
        }

        void Type(out BaseType type)
        {
            type = null;
            if (StartOf(11))
            {
                SingleType(out type);
            }
            else if (StartOf(12))
            {
                CollecType(out type);
            }
            else SynErr(61);
        }

        void StructStmt(out StmtNode stmt)
        {
            stmt = null;
            if (la.kind == 15)
            {
                StmtWhile(out stmt);
            }
            else if (la.kind == 16)
            {
                StmtFor(out stmt);
            }
            else if (la.kind == 17)
            {
                StmtForeach(out stmt);
            }
            else if (la.kind == 19)
            {
                StmtIf(out stmt);
            }
            else SynErr(62);
        }

        void SimpleStmt(out StmtNode stmt)
        {
            stmt = null;
            CallOrID(out IdentNode i);
            while (la.kind == 22)
            {
                Member(i, out IdentNode target);
                i = target;
            }
            stmt = new LoneCallNode(i);
            if (StartOf(13))
            {
                if (la.kind == 29)
                {
                    Assign(out ExprNode expr);
                    stmt = new AssignNode(i, expr);
                }
                else
                {
                    EdgeOneOrMore(i, out stmt);
                }
            }
        }

        void KeywordStmt(out StmtNode stmt)
        {
            stmt = null;
            if (la.kind == 26)
            {
                Get();
                Expr(out ExprNode expr);
                stmt = new ReturnNode(expr);
            }
            else if (la.kind == 27)
            {
                Get();
                stmt = new BreakNode();
            }
            else if (la.kind == 28)
            {
                Get();
                stmt = new ContinueNode();
            }
            else SynErr(63);
        }

        void FullDecl(out StmtNode stmt)
        {
            stmt = null; VertexDeclNode vertexDecl = null;
            Type(out BaseType type);
            if (la.kind == 1)
            {
                Get();
                string name = t.val; ExprNode expr = null;
                if (la.kind == 29)
                {
                    Assign(out expr);
                }
                stmt = new VarDeclNode(tab.NewObj(name, type, Var), expr);
            }
            else if (la.kind == 13)
            {
                Get();
                MultiDecl multiDecl = new MultiDecl();
                VtxDecl(out vertexDecl);
                multiDecl.AddDecl(vertexDecl);
                while (WeakSeparator(8, 14, 15))
                {
                    VtxDecl(out vertexDecl);
                    multiDecl.AddDecl(vertexDecl);
                }
                stmt = multiDecl;
                Expect(14);
            }
            else if (la.kind == 7)
            {
                VtxDecl(out vertexDecl);
                stmt = vertexDecl;
            }
            else SynErr(64);
        }

        void StmtWhile(out StmtNode w)
        {
            w = null; BlockNode stmtBlock = new BlockNode();
            Expect(15);
            ExprNode condition = null; StmtNode stmt = null;
            Expect(7);
            Expr(out condition);
            Expect(9);
            Expect(13);
            tab = tab.OpenScope();
            while (StartOf(1))
            {
                while (!(StartOf(2))) { SynErr(65); Get(); }
                Stmt(out stmt);
                stmtBlock.Add(stmt);
            }
            tab = tab.CloseScope();
            Expect(14);
            w = new WhileNode(condition, stmtBlock);
        }

        void StmtFor(out StmtNode f)
        {
            f = null; BlockNode stmtBlock = new BlockNode(); StmtNode stmt = null;
            Expect(16);
            StmtNode init = null; ExprNode cond = null; StmtNode iter = null;
            Expect(7);
            if (la.kind == 1)
            {
                SimpleStmt(out init);
            }
            else if (StartOf(4))
            {
                VarDecl(out VarDeclNode vert);
                init = vert;
            }
            else SynErr(66);
            ExpectWeak(8, 16);
            Expr(out cond);
            ExpectWeak(8, 3);
            Stmt(out iter);
            Expect(9);
            Expect(13);
            tab = tab.OpenScope();
            while (StartOf(1))
            {
                while (!(StartOf(2))) { SynErr(67); Get(); }
                Stmt(out stmt);
                stmtBlock.Add(stmt);
            }
            tab = tab.CloseScope();
            Expect(14);
            f = new ForNode(init, cond, iter, stmtBlock);
        }

        void StmtForeach(out StmtNode f)
        {
            f = null; StmtNode stmt = null;
            Expect(17);
            VarDeclNode itrVar = null; BlockNode stmtBlock = new BlockNode();
            Expect(7);
            tab = tab.OpenScope();
            Type(out BaseType type);
            Expect(1);
            itrVar = new VarDeclNode(tab.NewObj(t.val, type, Var), null);
            Expect(18);
            Expr(out ExprNode collection);
            Expect(9);
            Expect(13);
            while (StartOf(1))
            {
                while (!(StartOf(2))) { SynErr(68); Get(); }
                Stmt(out stmt);
                stmtBlock.Add(stmt);
            }
            tab = tab.CloseScope();
            Expect(14);
            f = new ForeachNode(itrVar, collection, stmtBlock);
        }

        void StmtIf(out StmtNode i)
        {
            i = null; ExprNode condition = null; BlockNode stmtBlock;
            Expect(19);
            Expect(7);
            StmtNode stmt = null; IfNode latestNode = null;
            Expr(out condition);
            Expect(9);
            Expect(13);
            tab = tab.OpenScope(); stmtBlock = new BlockNode();
            while (StartOf(1))
            {
                while (!(StartOf(2))) { SynErr(69); Get(); }
                Stmt(out stmt);
                stmtBlock.Add(stmt);
            }
            tab = tab.CloseScope();
            Expect(14);
            i = latestNode = new IfNode(condition, stmtBlock);
            while (la.kind == 20)
            {
                Get();
                Expect(7);
                Expr(out condition);
                Expect(9);
                Expect(13);
                tab = tab.OpenScope(); stmtBlock = new BlockNode();
                while (StartOf(1))
                {
                    while (!(StartOf(2))) { SynErr(70); Get(); }
                    Stmt(out stmt);
                    stmtBlock.Add(stmt);
                }
                tab = tab.CloseScope();
                Expect(14);
                latestNode.SetElse(new IfNode(condition, stmtBlock)); latestNode = latestNode.ElseNode;
            }
            if (la.kind == 21)
            {
                Get();
                Expect(13);
                tab = tab.OpenScope(); stmtBlock = new BlockNode();
                while (StartOf(1))
                {
                    while (!(StartOf(2))) { SynErr(71); Get(); }
                    Stmt(out stmt);
                    stmtBlock.Add(stmt);
                }
                tab = tab.CloseScope();
                Expect(14);
                latestNode.SetElse(new IfNode(null, stmtBlock));
            }
        }

        void Expr(out ExprNode e)
        {
            e = null;
            ExprOR(out e);
        }

        void CallOrID(out IdentNode i)
        {
            i = null;
            Identifier(out VarNode varNode);
            i = varNode;
            if (la.kind == 7)
            {
                Get();
                Args(out CollecConst collec);
                i = new CallNode(i.Ident, collec);
                Expect(9);
            }
        }

        void Member(ExprNode source, out IdentNode i)
        {
            i = null;
            ExpectWeak(22, 3);
            CallOrID(out i);
            i.Source = (IdentNode)source;
        }

        void Assign(out ExprNode expr)
        {
            expr = null; CollecConst collec = null;
            Expect(29);
            if (StartOf(17))
            {
                Expr(out expr);
            }
            else if (la.kind == 13)
            {
                Get();
                Args(out collec);
                expr = collec;
                Expect(14);
            }
            else SynErr(72);
        }

        void EdgeOneOrMore(IdentNode start, out StmtNode stmt)
        {
            stmt = null; EdgeCreateNode edge;
            EdgeOpr(out int op);
            stmt = edge = new EdgeCreateNode(start, op);
            if (la.kind == 1)
            {
                Identifier(out VarNode end);
                edge.AddRightSide(end, new List<AssignNode>());
            }
            else if (la.kind == 7)
            {
                EdgeCreate(edge);
            }
            else if (la.kind == 13)
            {
                Get();
                EdgeCreate(edge);
                while (WeakSeparator(8, 14, 15))
                {
                    EdgeCreate(edge);
                }
                Expect(14);
            }
            else SynErr(73);
        }

        void EdgeOpr(out int op)
        {
            op = 0;
            if (la.kind == 23)
            {
                Get();
                op = Operators.Leftarr;
            }
            else if (la.kind == 24)
            {
                Get();
                op = Operators.Nonarr;
            }
            else if (la.kind == 25)
            {
                Get();
                op = Operators.Rightarr;
            }
            else SynErr(74);
        }

        void Identifier(out VarNode varNode)
        {
            varNode = null;
            Expect(1);
            varNode = new VarNode(t.val);
        }

        void EdgeCreate(EdgeCreateNode edge)
        {
            VarNode varNode = null; ExprNode expr = null; List<AssignNode> attributes = new List<AssignNode>();
            Expect(7);
            Identifier(out VarNode right);
            while (WeakSeparator(8, 18, 5))
            {
                Identifier(out varNode);
                Assign(out expr);
                attributes.Add(new AssignNode(varNode, expr));
            }
            edge.AddRightSide(right, attributes);
            Expect(9);
        }

        void VtxDecl(out VertexDeclNode vertexDecl)
        {
            vertexDecl = null; VarNode varNode = null; ExprNode expr = null;
            Expect(7);
            Expect(1);
            vertexDecl = new VertexDeclNode(tab.NewObj(t.val, new BaseType("vertex"), Var));
            while (WeakSeparator(8, 18, 5))
            {
                Identifier(out varNode);
                Assign(out expr);
                vertexDecl.AddAttr(new AssignNode(varNode, expr));
            }
            Expect(9);
        }

        void Args(out CollecConst collec)
        {
            collec = new CollecConst(); ExprNode expr;
            if (StartOf(17))
            {
                Expr(out expr);
                collec.Add(expr);
                while (WeakSeparator(8, 17, 19))
                {
                    Expr(out expr);
                    collec.Add(expr);
                }
            }
        }

        void ExprOR(out ExprNode e)
        {
            e = null; int op = 0;
            ExprAnd(out ExprNode e1);
            e = e1;
            while (la.kind == 30)
            {
                Get();
                op = Operators.Or;
                ExprAnd(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprAnd(out ExprNode e)
        {
            e = null; int op = 0;
            ExprEQ(out ExprNode e1);
            e = e1;
            while (la.kind == 31)
            {
                Get();
                op = Operators.And;
                ExprEQ(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprEQ(out ExprNode e)
        {
            e = null; int op = 0;
            ExprRel(out ExprNode e1);
            e = e1;
            if (la.kind == 32 || la.kind == 33)
            {
                if (la.kind == 32)
                {
                    Get();
                    op = Operators.Eq;
                }
                else
                {
                    Get();
                    op = Operators.Neq;
                }
                ExprRel(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprRel(out ExprNode e)
        {
            e = null; int op = 0;
            ExprPlus(out ExprNode e1);
            e = e1;
            if (StartOf(20))
            {
                if (la.kind == 34)
                {
                    Get();
                    op = Operators.Less;
                }
                else if (la.kind == 35)
                {
                    Get();
                    op = Operators.Greater;
                }
                else if (la.kind == 36)
                {
                    Get();
                    op = Operators.Lesseq;
                }
                else
                {
                    Get();
                    op = Operators.Greateq;
                }
                ExprPlus(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprPlus(out ExprNode e)
        {
            e = null; bool b = false; int op = 0;
            if (la.kind == 38)
            {
                Get();
                b = true;
            }
            ExprMult(out ExprNode e1);
            if (b) e = new UnaExprNode(Operators.Umin, e1); else e = e1;
            while (la.kind == 38 || la.kind == 39)
            {
                if (la.kind == 39)
                {
                    Get();
                    op = Operators.Plus;
                }
                else
                {
                    Get();
                    op = Operators.Bimin;
                }
                ExprMult(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprMult(out ExprNode e)
        {
            e = null; int op = 0;
            ExprNot(out ExprNode e1);
            e = e1;
            while (la.kind == 40 || la.kind == 41 || la.kind == 42)
            {
                if (la.kind == 40)
                {
                    Get();
                    op = Operators.Mult;
                }
                else if (la.kind == 41)
                {
                    Get();
                    op = Operators.Div;
                }
                else
                {
                    Get();
                    op = Operators.Mod;
                }
                ExprNot(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprNot(out ExprNode e)
        {
            e = null; bool b = false;
            if (la.kind == 43)
            {
                Get();
                b = true;
            }
            Factor(out e);
            if (b) e = new UnaExprNode(Operators.Not, e);
        }

        void Factor(out ExprNode e)
        {
            e = null;
            if (StartOf(21))
            {
                Const(out e);
            }
            else if (la.kind == 1 || la.kind == 7)
            {
                if (la.kind == 7)
                {
                    Get();
                    Expr(out e);
                    e.InParentheses = true;
                    Expect(9);
                }
                else
                {
                    CallOrID(out IdentNode ident);
                    e = ident;
                }
                while (la.kind == 22)
                {
                    Member(e, out IdentNode target);
                    e = target;
                }
            }
            else SynErr(75);
        }

        void Const(out ExprNode e)
        {
            e = null;
            if (la.kind == 2)
            {
                Get();
                e = new NumConst(Convert.ToDouble(t.val, System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (la.kind == 3)
            {
                Get();
                e = new TextConst(t.val);
            }
            else if (la.kind == 44)
            {
                Get();
                e = new BoolConst(Convert.ToBoolean(t.val));
            }
            else if (la.kind == 45)
            {
                Get();
                e = new BoolConst(Convert.ToBoolean(t.val));
            }
            else if (la.kind == 12)
            {
                Get();
                e = new NoneConst();
            }
            else SynErr(76);
        }

        void SingleType(out BaseType type)
        {
            type = null;
            if (la.kind == 50)
            {
                Get();
                type = new BaseType("number");
            }
            else if (la.kind == 51)
            {
                Get();
                type = new BaseType("boolean");
            }
            else if (la.kind == 52)
            {
                Get();
                type = new BaseType("text");
            }
            else if (la.kind == 5)
            {
                Get();
                type = new BaseType("vertex");
            }
            else if (la.kind == 6)
            {
                Get();
                type = new BaseType("edge");
            }
            else SynErr(77);
        }

        void CollecType(out BaseType type)
        {
            type = null; BaseType subType = null;
            if (la.kind == 46)
            {
                Get();
                ExpectWeak(34, 3);
                SingleType(out subType);
                ExpectWeak(35, 22);
                type = new BaseType(subType, new BaseType("list"));
            }
            else if (la.kind == 47)
            {
                Get();
                ExpectWeak(34, 3);
                SingleType(out subType);
                ExpectWeak(35, 22);
                type = new BaseType(subType, new BaseType("set"));
            }
            else if (la.kind == 48)
            {
                Get();
                ExpectWeak(34, 3);
                SingleType(out subType);
                ExpectWeak(35, 22);
                type = new BaseType(subType, new BaseType("queue"));
            }
            else if (la.kind == 49)
            {
                Get();
                ExpectWeak(34, 3);
                SingleType(out subType);
                ExpectWeak(35, 22);
                type = new BaseType(subType, new BaseType("stack"));
            }
            else SynErr(78);
        }



        public void Parse()
        {
            la = new Token();
            la.val = "";
            Get();
            MAGIA();
            Expect(0);

        }

        static readonly bool[,] set = {
        {_T,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_T, _x,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_x,_T,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_T,_T,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_T,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_T, _x,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_x,_x,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_T,_T,_x,_x, _T,_T,_T,_x, _x,_x,_T,_T, _x,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_T,_T,_x,_x, _T,_T,_T,_x, _x,_T,_x,_T, _x,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_T,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_T, _x,_T,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_T,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_T, _x,_x,_T,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_x,_x,_x,_x, _x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_T,_T,_T,_T, _T,_T,_T,_T, _x,_x,_x,_T, _T,_x,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_T, _T,_T,_T,_T, _T,_T,_T,_T, _T,_x,_x},
        {_x,_T,_T,_T, _x,_x,_x,_T, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_x,_x,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x},
        {_T,_T,_x,_x, _T,_T,_T,_T, _x,_x,_x,_T, _x,_T,_x,_T, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x}

    };
    } // end Parser


    public class Errors
    {
        public int count = 0;                                    // number of errors detected
        public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
        public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

        public virtual void SynErr(int line, int col, int n)
        {
            string s;
            switch (n)
            {
                case 0: s = "EOF expected"; break;
                case 1: s = "IDENT expected"; break;
                case 2: s = "NUMBER expected"; break;
                case 3: s = "TEXT expected"; break;
                case 4: s = "\"[\" expected"; break;
                case 5: s = "\"vertex\" expected"; break;
                case 6: s = "\"edge\" expected"; break;
                case 7: s = "\"(\" expected"; break;
                case 8: s = "\",\" expected"; break;
                case 9: s = "\")\" expected"; break;
                case 10: s = "\"]\" expected"; break;
                case 11: s = "\"func\" expected"; break;
                case 12: s = "\"none\" expected"; break;
                case 13: s = "\"{\" expected"; break;
                case 14: s = "\"}\" expected"; break;
                case 15: s = "\"while\" expected"; break;
                case 16: s = "\"for\" expected"; break;
                case 17: s = "\"foreach\" expected"; break;
                case 18: s = "\"in\" expected"; break;
                case 19: s = "\"if\" expected"; break;
                case 20: s = "\"elseif\" expected"; break;
                case 21: s = "\"else\" expected"; break;
                case 22: s = "\".\" expected"; break;
                case 23: s = "\"<-\" expected"; break;
                case 24: s = "\"--\" expected"; break;
                case 25: s = "\"->\" expected"; break;
                case 26: s = "\"return\" expected"; break;
                case 27: s = "\"break\" expected"; break;
                case 28: s = "\"continue\" expected"; break;
                case 29: s = "\"=\" expected"; break;
                case 30: s = "\"||\" expected"; break;
                case 31: s = "\"&&\" expected"; break;
                case 32: s = "\"==\" expected"; break;
                case 33: s = "\"!=\" expected"; break;
                case 34: s = "\"<\" expected"; break;
                case 35: s = "\">\" expected"; break;
                case 36: s = "\"<=\" expected"; break;
                case 37: s = "\">=\" expected"; break;
                case 38: s = "\"-\" expected"; break;
                case 39: s = "\"+\" expected"; break;
                case 40: s = "\"*\" expected"; break;
                case 41: s = "\"/\" expected"; break;
                case 42: s = "\"%\" expected"; break;
                case 43: s = "\"!\" expected"; break;
                case 44: s = "\"true\" expected"; break;
                case 45: s = "\"false\" expected"; break;
                case 46: s = "\"list\" expected"; break;
                case 47: s = "\"set\" expected"; break;
                case 48: s = "\"queue\" expected"; break;
                case 49: s = "\"stack\" expected"; break;
                case 50: s = "\"number\" expected"; break;
                case 51: s = "\"boolean\" expected"; break;
                case 52: s = "\"text\" expected"; break;
                case 53: s = "??? expected"; break;
                case 54: s = "this symbol not expected in MAGIA"; break;
                case 55: s = "this symbol not expected in MAGIA"; break;
                case 56: s = "this symbol not expected in MAGIA"; break;
                case 57: s = "invalid Head"; break;
                case 58: s = "invalid Stmt"; break;
                case 59: s = "invalid FuncDecl"; break;
                case 60: s = "this symbol not expected in FuncDecl"; break;
                case 61: s = "invalid Type"; break;
                case 62: s = "invalid StructStmt"; break;
                case 63: s = "invalid KeywordStmt"; break;
                case 64: s = "invalid FullDecl"; break;
                case 65: s = "this symbol not expected in StmtWhile"; break;
                case 66: s = "invalid StmtFor"; break;
                case 67: s = "this symbol not expected in StmtFor"; break;
                case 68: s = "this symbol not expected in StmtForeach"; break;
                case 69: s = "this symbol not expected in StmtIf"; break;
                case 70: s = "this symbol not expected in StmtIf"; break;
                case 71: s = "this symbol not expected in StmtIf"; break;
                case 72: s = "invalid Assign"; break;
                case 73: s = "invalid EdgeOneOrMore"; break;
                case 74: s = "invalid EdgeOpr"; break;
                case 75: s = "invalid Factor"; break;
                case 76: s = "invalid Const"; break;
                case 77: s = "invalid SingleType"; break;
                case 78: s = "invalid CollecType"; break;

                default: s = "error " + n; break;
            }
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(string s)
        {
            errorStream.WriteLine(s);
            count++;
        }

        public virtual void Warning(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
        }

        public virtual void Warning(string s)
        {
            errorStream.WriteLine(s);
        }
    } // Errors


    public class FatalError : Exception
    {
        public FatalError(string m) : base(m) { }
    }

    class BaseTypeList : List<BaseType>
    { }
}