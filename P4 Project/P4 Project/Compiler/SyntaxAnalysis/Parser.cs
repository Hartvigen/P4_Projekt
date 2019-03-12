
using System;

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
        public const int maxT = 36;

        const bool T = true;
        const bool x = false;
        const int minErrDist = 2;

        public Scanner scanner;
        public Errors errors;

        public Token t;    // last recognized token
        public Token la;   // lookahead token
        int errDist = minErrDist;

        const int // types
                undef = 0, number = 1, boolean = 2, text = 3, vertex = 4, edge = 5, setType = 10, list = 20, queue = 30, stack = 40;


        const int // object kinds
            var = 0, proc = 1;

        /*
        public SymbolTable   tab;
        public CodeGenerator gen;
        */

        /*--------------------------------------------------------------------------*/


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
            while (la.kind == 7)
            {
                while (!(la.kind == 0 || la.kind == 7)) { SynErr(37); Get(); }
                Get();
                Head();
                Expect(8);
            }
        }

        void Head()
        {
            if (la.kind == 9)
            {
                Get();
            }
            else if (la.kind == 10)
            {
                Get();
            }
            else SynErr(38);
            Expect(11);
            AttrDecls();
            Expect(12);
        }

        void AttrDecls()
        {
            AttrDecl();
            while (WeakSeparator(13, 1, 2))
            {
                AttrDecl();
            }
        }

        void AttrDecl()
        {
            Type();
            Expect(1);
            if (la.kind == 14)
            {
                Get();
                Expr();
            }
        }

        void Type()
        {
            if (StartOf(3))
            {
                SingleType();
            }
            else if (StartOf(4))
            {
                CollecType();
            }
            else SynErr(39);
        }

        void Expr()
        {
            ExprOR();
        }

        void ExprOR()
        {
            ExprAnd();
            while (la.kind == 15)
            {
                Get();
                ExprAnd();
            }
        }

        void ExprAnd()
        {
            ExprEQ();
            while (la.kind == 16)
            {
                Get();
                ExprEQ();
            }
        }

        void ExprEQ()
        {
            ExprRel();
            if (la.kind == 17 || la.kind == 18)
            {
                if (la.kind == 17)
                {
                    Get();
                }
                else
                {
                    Get();
                }
                ExprRel();
            }
        }

        void ExprRel()
        {
            ExprPlus();
            if (StartOf(5))
            {
                if (la.kind == 19)
                {
                    Get();
                }
                else if (la.kind == 20)
                {
                    Get();
                }
                else if (la.kind == 21)
                {
                    Get();
                }
                else
                {
                    Get();
                }
                ExprPlus();
            }
        }

        void ExprPlus()
        {
            if (la.kind == 23)
            {
                Get();
            }
            ExprMult();
            while (la.kind == 23 || la.kind == 24)
            {
                if (la.kind == 24)
                {
                    Get();
                }
                else
                {
                    Get();
                }
                ExprMult();
            }
        }

        void ExprMult()
        {
            ExprNot();
            while (la.kind == 25 || la.kind == 26 || la.kind == 27)
            {
                if (la.kind == 25)
                {
                    Get();
                }
                else if (la.kind == 26)
                {
                    Get();
                }
                else
                {
                    Get();
                }
                ExprNot();
            }
        }

        void ExprNot()
        {
            if (la.kind == 28)
            {
                Get();
            }
            Factor();
        }

        void Factor()
        {
            if (la.kind == 11)
            {
                Get();
                Expr();
                Expect(12);
            }
            else if (StartOf(6))
            {
                Const();
            }
            else if (la.kind == 1)
            {
                CallOrID();
            }
            else SynErr(40);
        }

        void Const()
        {
            if (la.kind == 2)
            {
                Get();
            }
            else if (la.kind == 3)
            {
                Get();
            }
            else if (la.kind == 5)
            {
                Get();
            }
            else if (la.kind == 6)
            {
                Get();
            }
            else if (la.kind == 4)
            {
                Get();
            }
            else SynErr(41);
        }

        void CallOrID()
        {
            Expect(1);
            if (la.kind == 11)
            {
                Get();
                Args();
                Expect(12);
            }
        }

        void Args()
        {
            if (StartOf(7))
            {
                Expr();
                while (WeakSeparator(13, 7, 2))
                {
                    Expr();
                }
            }
        }

        void SingleType()
        {
            if (la.kind == 33)
            {
                Get();
            }
            else if (la.kind == 34)
            {
                Get();
            }
            else if (la.kind == 35)
            {
                Get();
            }
            else if (la.kind == 9)
            {
                Get();
            }
            else if (la.kind == 10)
            {
                Get();
            }
            else SynErr(42);
        }

        void CollecType()
        {
            if (la.kind == 29)
            {
                Get();
                ExpectWeak(19, 8);
                SingleType();
                ExpectWeak(20, 9);
            }
            else if (la.kind == 30)
            {
                Get();
                ExpectWeak(19, 8);
                SingleType();
                ExpectWeak(20, 9);
            }
            else if (la.kind == 31)
            {
                Get();
                ExpectWeak(19, 8);
                SingleType();
                ExpectWeak(20, 9);
            }
            else if (la.kind == 32)
            {
                Get();
                ExpectWeak(19, 8);
                SingleType();
                ExpectWeak(20, 9);
            }
            else SynErr(43);
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
        {T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, x,x},
        {x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x},
        {x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,x,x,x, x,x},
        {x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
        {x,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
        {T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x},
        {T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

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
                case 7: s = "\"[\" expected"; break;
                case 8: s = "\"]\" expected"; break;
                case 9: s = "\"vertex\" expected"; break;
                case 10: s = "\"edge\" expected"; break;
                case 11: s = "\"(\" expected"; break;
                case 12: s = "\")\" expected"; break;
                case 13: s = "\",\" expected"; break;
                case 14: s = "\"=\" expected"; break;
                case 15: s = "\"||\" expected"; break;
                case 16: s = "\"&&\" expected"; break;
                case 17: s = "\"==\" expected"; break;
                case 18: s = "\"!=\" expected"; break;
                case 19: s = "\"<\" expected"; break;
                case 20: s = "\">\" expected"; break;
                case 21: s = "\"<=\" expected"; break;
                case 22: s = "\">=\" expected"; break;
                case 23: s = "\"-\" expected"; break;
                case 24: s = "\"+\" expected"; break;
                case 25: s = "\"*\" expected"; break;
                case 26: s = "\"/\" expected"; break;
                case 27: s = "\"%\" expected"; break;
                case 28: s = "\"!\" expected"; break;
                case 29: s = "\"list\" expected"; break;
                case 30: s = "\"set\" expected"; break;
                case 31: s = "\"queue\" expected"; break;
                case 32: s = "\"stack\" expected"; break;
                case 33: s = "\"number\" expected"; break;
                case 34: s = "\"bool\" expected"; break;
                case 35: s = "\"text\" expected"; break;
                case 36: s = "??? expected"; break;
                case 37: s = "this symbol not expected in MAGIA"; break;
                case 38: s = "invalid Head"; break;
                case 39: s = "invalid Type"; break;
                case 40: s = "invalid Factor"; break;
                case 41: s = "invalid Const"; break;
                case 42: s = "invalid SingleType"; break;
                case 43: s = "invalid CollecType"; break;

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