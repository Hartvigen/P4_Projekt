
using System;
using P4_Project.AST;
using P4_Project.AST.Commands;
using P4_Project.AST.Commands.Stmts;
using P4_Project.AST.Commands.Stmts.Decls;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Values;

namespace P4_Project.Compiler.SyntaxAnalysis
{



    public class Parser
    {
        public const int _EOF = 0;
        public const int _IDENT = 1;
        public const int _NUMBER = 2;
        public const int _TEXT = 3;
        public const int _NONE = 4;
        public const int _TRUE = 5;
        public const int _FALSE = 6;
        public const int _RPAREN = 7;
        public const int maxT = 44;

        const bool T = true;
        const bool x = false;
        const int minErrDist = 2;

        public Scanner scanner;
        public Errors errors;

        public Token t;    // last recognized token
        public Token la;   // lookahead token
        int errDist = minErrDist;

        public MAGIA mainNode;



        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            errors = new Errors();
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
            mainNode = null; Block mainBlock = new Block();
            while (la.kind == 8)
            {
                while (!(la.kind == 0 || la.kind == 8)) { SynErr(45); Get(); }
                Head(out HeadNode headNode);
                mainBlock.Add(headNode);
            }
            Stmts(ref mainBlock);
            while (la.kind == 14)
            {
                FuncDecl(out FuncDeclNode funcNode);
                mainBlock.Add(funcNode);
            }
            mainNode = new MAGIA(mainBlock);
        }

        void Head(out HeadNode headNode)
        {
            headNode = null;
            Expect(8);
            if (la.kind == 9)
            {
                Get();
                headNode = new HeadNode(HeadNode.VERTEX);
            }
            else if (la.kind == 10)
            {
                Get();
                headNode = new HeadNode(HeadNode.EDGE);
            }
            else SynErr(46);
            ExpectWeak(11, 1);
            AttrDecls(ref headNode);
            ExpectWeak(7, 2);
            Expect(12);
        }

        void Stmts(ref Block block)
        {
            StmtNode stmtNode;
            while (!(StartOf(3))) { SynErr(47); Get(); }
            while (StartOf(4))
            {
                Stmt(out stmtNode);
                block.Add(stmtNode);
            }
        }

        void FuncDecl(out FuncDeclNode funcNode)
        {
            funcNode = null; string funcName = ""; Block paramBlock = new Block(); Block stmtBlock = new Block();
            while (!(la.kind == 0 || la.kind == 14)) { SynErr(48); Get(); }
            Expect(14);
            Expect(1);
            funcName = t.val;
            Expect(11);
            if (la.kind != _RPAREN)
            {
                FuncParams(ref paramBlock);
            }
            Expect(7);
            while (!(la.kind == 0 || la.kind == 15)) { SynErr(49); Get(); }
            Expect(15);
            Stmts(ref stmtBlock);
            while (!(la.kind == 0 || la.kind == 16)) { SynErr(50); Get(); }
            Expect(16);
            funcNode = new FuncDeclNode(funcName, paramBlock, stmtBlock);
        }

        void AttrDecls(ref HeadNode headNode)
        {
            VarDeclNode varDecl;
            AttrDecl(out varDecl);
            headNode?.AddAttr(varDecl);
            while (WeakSeparator(13, 5, 6))
            {
                AttrDecl(out varDecl);
                headNode?.AddAttr(varDecl);
            }
        }

        void AttrDecl(out VarDeclNode varDecl)
        {
            varDecl = null;
            Type(out int typ);
            Expect(1);
            if (la.kind == 19)
            {
                Assign(out AssignNode assign);
            }
        }

        void Type(out int type)
        {
            type = 0;
            if (StartOf(7))
            {
                SingleType(out type);
            }
            else if (StartOf(8))
            {
                CollecType(out type);
            }
            else SynErr(51);
        }

        void Assign(out AssignNode assign)
        {
            assign = null;
            Expect(19);
            if (StartOf(9))
            {
                Expr(out ExprNode expr);
                assign = new AssignNode();
            }
            else if (la.kind == 15)
            {
                Get();
                Args(out CollecConst collec);
                assign = new AssignNode();
                Expect(16);
            }
            else SynErr(52);
        }

        void FuncParams(ref Block paramBlock)
        {
            int typ = 0;
            Type(out typ);
            Expect(1);
            paramBlock.Add(new VarDeclNode(typ, t.val, null));
            while (la.kind == 13)
            {
                while (!(la.kind == 0 || la.kind == 13)) { SynErr(53); Get(); }
                Get();
                Type(out typ);
                Expect(1);
                paramBlock.Add(new VarDeclNode(typ, t.val, null));
            }
        }

        void Stmt(out StmtNode stmtNode)
        {
            stmtNode = null;
            if (StartOf(5))
            {
                FullDecl();
            }
            else if (la.kind == 1)
            {
                CallOrID(out IdentNode i);
                while (la.kind == 18)
                {
                    Member();
                }
                IdentCont();
            }
            else if (la.kind == 17)
            {
                Get();
                Expr(out ExprNode expr);
            }
            else SynErr(54);
        }

        void FullDecl()
        {
            Type(out int typ);
            if (la.kind == 1)
            {
                Get();
                if (la.kind == 19)
                {
                    Assign(out AssignNode assign);
                }
            }
            else if (la.kind == 15)
            {
                Get();
                VtxDecls();
                Expect(16);
            }
            else if (la.kind == 11)
            {
                VtxDecl();
            }
            else SynErr(55);
        }

        void CallOrID(out IdentNode i)
        {
            Identifier(out i);
            if (la.kind == 11)
            {
                Get();
                Args(out CollecConst collec);
                Expect(7);
            }
        }

        void Member()
        {
            ExpectWeak(18, 1);
            CallOrID(out IdentNode i);
        }

        void IdentCont()
        {
            if (la.kind == 19)
            {
                Assign(out AssignNode assign);
            }
            else if (la.kind == 20 || la.kind == 21 || la.kind == 22)
            {
                EdgeOpr();
                EdgeOneOrMore();
            }
            else SynErr(56);
        }

        void Expr(out ExprNode e)
        {
            e = null;
            ExprOR(out e);
        }

        void EdgeOpr()
        {
            if (la.kind == 20)
            {
                Get();
            }
            else if (la.kind == 21)
            {
                Get();
            }
            else if (la.kind == 22)
            {
                Get();
            }
            else SynErr(57);
        }

        void EdgeOneOrMore()
        {
            if (la.kind == 1)
            {
                Identifier(out IdentNode identNode);
            }
            else if (la.kind == 11)
            {
                EdgeDecl();
            }
            else if (la.kind == 15)
            {
                Get();
                EdgeDecls();
                Expect(16);
            }
            else SynErr(58);
        }

        void VtxDecls()
        {
            VtxDecl();
            while (WeakSeparator(13, 10, 11))
            {
                VtxDecl();
            }
        }

        void VtxDecl()
        {
            Expect(11);
            Expect(1);
            VEParams();
            Expect(7);
        }

        void Args(out CollecConst collec)
        {
            collec = new CollecConst(); ExprNode expr;
            if (StartOf(9))
            {
                Expr(out expr);
                collec.Add(expr);
                while (WeakSeparator(13, 9, 12))
                {
                    Expr(out expr);
                    collec.Add(expr);
                }
            }
        }

        void VEParams()
        {
            while (WeakSeparator(13, 13, 6))
            {
                Identifier(out IdentNode identNode);
                Assign(out AssignNode assign);
            }
        }

        void Identifier(out IdentNode identNode)
        {
            identNode = null;
            Expect(1);
            identNode = new IdentNode();
        }

        void EdgeDecl()
        {
            Expect(11);
            Expect(1);
            VEParams();
            Expect(7);
        }

        void EdgeDecls()
        {
            EdgeDecl();
            while (WeakSeparator(13, 10, 11))
            {
                EdgeDecl();
            }
        }

        void ExprOR(out ExprNode e)
        {
            e = null; int op = 0;
            ExprAnd(out ExprNode e1);
            e = e1;
            while (la.kind == 23)
            {
                Get();
                op = Operators.OR;
                ExprAnd(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprAnd(out ExprNode e)
        {
            e = null; int op = 0;
            ExprEQ(out ExprNode e1);
            e = e1;
            while (la.kind == 24)
            {
                Get();
                op = Operators.EQ;
                ExprEQ(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprEQ(out ExprNode e)
        {
            e = null; int op = 0;
            ExprRel(out ExprNode e1);
            e = e1;
            if (la.kind == 25 || la.kind == 26)
            {
                if (la.kind == 25)
                {
                    Get();
                    op = Operators.EQ;
                }
                else
                {
                    Get();
                    op = Operators.NEQ;
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
            if (StartOf(14))
            {
                if (la.kind == 27)
                {
                    Get();
                    op = Operators.LESS;
                }
                else if (la.kind == 28)
                {
                    Get();
                    op = Operators.GREATER;
                }
                else if (la.kind == 29)
                {
                    Get();
                    op = Operators.LESSEQ;
                }
                else
                {
                    Get();
                    op = Operators.GREATEQ;
                }
                ExprPlus(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprPlus(out ExprNode e)
        {
            e = null; bool b = false; int op = 0;
            if (la.kind == 31)
            {
                Get();
                b = true;
            }
            ExprMult(out ExprNode e1);
            if (b) e = new UnaExprNode(Operators.UMIN, e1); else e = e1;
            while (la.kind == 31 || la.kind == 32)
            {
                if (la.kind == 32)
                {
                    Get();
                    op = Operators.PLUS;
                }
                else
                {
                    Get();
                    op = Operators.BIMIN;
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
            while (la.kind == 33 || la.kind == 34 || la.kind == 35)
            {
                if (la.kind == 33)
                {
                    Get();
                    op = Operators.MULT;
                }
                else if (la.kind == 34)
                {
                    Get();
                    op = Operators.DIV;
                }
                else
                {
                    Get();
                    op = Operators.MOD;
                }
                ExprNot(out ExprNode e2);
                e = new BinExprNode(e, op, e2);
            }
        }

        void ExprNot(out ExprNode e)
        {
            e = null; bool b = false;
            if (la.kind == 36)
            {
                Get();
                b = true;
            }
            Factor(out e);
            if (b) e = new UnaExprNode(Operators.NOT, e);
        }

        void Factor(out ExprNode e)
        {
            e = null;
            if (StartOf(15))
            {
                Const(out e);
            }
            else if (la.kind == 1 || la.kind == 11)
            {
                if (la.kind == 11)
                {
                    Get();
                    Expr(out e);
                    Expect(7);
                }
                else
                {
                    CallOrID(out IdentNode i);
                }
                while (la.kind == 18)
                {
                    Member();
                }
            }
            else SynErr(59);
        }

        void Const(out ExprNode e)
        {
            e = null;
            if (la.kind == 2)
            {
                Get();
                e = new NumConst(Convert.ToInt32(t.val));
            }
            else if (la.kind == 3)
            {
                Get();
                e = new TextConst(t.val);
            }
            else if (la.kind == 5)
            {
                Get();
                e = new BoolConst(Convert.ToBoolean(t.val));
            }
            else if (la.kind == 6)
            {
                Get();
                e = new BoolConst(Convert.ToBoolean(t.val));
            }
            else if (la.kind == 4)
            {
                Get();
                e = new NoneConst();
            }
            else SynErr(60);
        }

        void SingleType(out int type)
        {
            type = 0;
            if (la.kind == 41)
            {
                Get();
                type = Types.number;
            }
            else if (la.kind == 42)
            {
                Get();
                type = Types.boolean;
            }
            else if (la.kind == 43)
            {
                Get();
                type = Types.text;
            }
            else if (la.kind == 9)
            {
                Get();
                type = Types.vertex;
            }
            else if (la.kind == 10)
            {
                Get();
                type = Types.edge;
            }
            else SynErr(61);
        }

        void CollecType(out int type)
        {
            type = 0; int subType = 0;
            if (la.kind == 37)
            {
                Get();
                ExpectWeak(27, 1);
                SingleType(out subType);
                ExpectWeak(28, 16);
                type = Types.list + subType;
            }
            else if (la.kind == 38)
            {
                Get();
                ExpectWeak(27, 1);
                SingleType(out subType);
                ExpectWeak(28, 16);
                type = Types.set + subType;
            }
            else if (la.kind == 39)
            {
                Get();
                ExpectWeak(27, 1);
                SingleType(out subType);
                ExpectWeak(28, 16);
                type = Types.queue + subType;
            }
            else if (la.kind == 40)
            {
                Get();
                ExpectWeak(27, 1);
                SingleType(out subType);
                ExpectWeak(28, 16);
                type = Types.stack + subType;
            }
            else SynErr(62);
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
        {T,T,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x},
        {T,T,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x},
        {T,T,x,x, x,x,x,x, T,T,T,x, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x},
        {T,T,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x},
        {x,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x},
        {x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x},
        {x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x},
        {x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,x,x,x, x,x},
        {x,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
        {x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {T,T,x,x, x,x,x,x, T,T,T,T, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x}

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
                case 4: s = "NONE expected"; break;
                case 5: s = "TRUE expected"; break;
                case 6: s = "FALSE expected"; break;
                case 7: s = "RPAREN expected"; break;
                case 8: s = "\"[\" expected"; break;
                case 9: s = "\"vertex\" expected"; break;
                case 10: s = "\"edge\" expected"; break;
                case 11: s = "\"(\" expected"; break;
                case 12: s = "\"]\" expected"; break;
                case 13: s = "\",\" expected"; break;
                case 14: s = "\"func\" expected"; break;
                case 15: s = "\"{\" expected"; break;
                case 16: s = "\"}\" expected"; break;
                case 17: s = "\"return\" expected"; break;
                case 18: s = "\".\" expected"; break;
                case 19: s = "\"=\" expected"; break;
                case 20: s = "\"<-\" expected"; break;
                case 21: s = "\"--\" expected"; break;
                case 22: s = "\"->\" expected"; break;
                case 23: s = "\"||\" expected"; break;
                case 24: s = "\"&&\" expected"; break;
                case 25: s = "\"==\" expected"; break;
                case 26: s = "\"!=\" expected"; break;
                case 27: s = "\"<\" expected"; break;
                case 28: s = "\">\" expected"; break;
                case 29: s = "\"<=\" expected"; break;
                case 30: s = "\">=\" expected"; break;
                case 31: s = "\"-\" expected"; break;
                case 32: s = "\"+\" expected"; break;
                case 33: s = "\"*\" expected"; break;
                case 34: s = "\"/\" expected"; break;
                case 35: s = "\"%\" expected"; break;
                case 36: s = "\"!\" expected"; break;
                case 37: s = "\"list\" expected"; break;
                case 38: s = "\"set\" expected"; break;
                case 39: s = "\"queue\" expected"; break;
                case 40: s = "\"stack\" expected"; break;
                case 41: s = "\"number\" expected"; break;
                case 42: s = "\"bool\" expected"; break;
                case 43: s = "\"text\" expected"; break;
                case 44: s = "??? expected"; break;
                case 45: s = "this symbol not expected in MAGIA"; break;
                case 46: s = "invalid Head"; break;
                case 47: s = "this symbol not expected in Stmts"; break;
                case 48: s = "this symbol not expected in FuncDecl"; break;
                case 49: s = "this symbol not expected in FuncDecl"; break;
                case 50: s = "this symbol not expected in FuncDecl"; break;
                case 51: s = "invalid Type"; break;
                case 52: s = "invalid Assign"; break;
                case 53: s = "this symbol not expected in FuncParams"; break;
                case 54: s = "invalid Stmt"; break;
                case 55: s = "invalid FullDecl"; break;
                case 56: s = "invalid IdentCont"; break;
                case 57: s = "invalid EdgeOpr"; break;
                case 58: s = "invalid EdgeOneOrMore"; break;
                case 59: s = "invalid Factor"; break;
                case 60: s = "invalid Const"; break;
                case 61: s = "invalid SingleType"; break;
                case 62: s = "invalid CollecType"; break;

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
}