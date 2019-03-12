
using System;

namespace P4_Project.Compiler.SyntaxAnalysis {



public class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _NUMBER = 2;
	public const int _TEXT = 3;
	public const int _NONE = 4;
	public const int _TRUE = 5;
	public const int _FALSE = 6;
	public const int maxT = 51;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

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


	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void MAGIA() {
		while (la.kind == 7) {
			while (!(la.kind == 0 || la.kind == 7)) {SynErr(52); Get();}
			Get();
			Head();
			Expect(8);
		}
		Stmts();
		while (la.kind == 14) {
			FuncDecl();
		}
	}

	void Head() {
		if (la.kind == 9) {
			Get();
		} else if (la.kind == 10) {
			Get();
		} else SynErr(53);
		Expect(11);
		AttrDecls();
		Expect(12);
	}

	void Stmts() {
		while (!(StartOf(1))) {SynErr(54); Get();}
		while (StartOf(2)) {
			if (StartOf(3)) {
				Stmt();
			} else {
				StrucStmt();
			}
		}
	}

	void FuncDecl() {
		while (!(la.kind == 0 || la.kind == 14)) {SynErr(55); Get();}
		Expect(14);
		Expect(1);
		Expect(11);
		FuncParam();
		Expect(12);
		Expect(15);
		Stmts();
		Expect(16);
	}

	void AttrDecls() {
		AttrDecl();
		while (WeakSeparator(13,4,5) ) {
			AttrDecl();
		}
	}

	void AttrDecl() {
		Type();
		Expect(1);
		if (la.kind == 26) {
			Assign();
		}
	}

	void Type() {
		if (StartOf(6)) {
			SingleType();
		} else if (StartOf(7)) {
			CollecType();
		} else SynErr(56);
	}

	void Assign() {
		Expect(26);
		if (StartOf(8)) {
			Expr();
		} else if (la.kind == 15) {
			Get();
			Args();
			Expect(16);
		} else SynErr(57);
	}

	void FuncParam() {
		if (StartOf(4)) {
			HalfDecl();
			while (WeakSeparator(13,4,5) ) {
				HalfDecl();
			}
		}
	}

	void HalfDecl() {
		Type();
		Expect(1);
	}

	void Stmt() {
		if (StartOf(4)) {
			FullDecl();
		} else if (la.kind == 1) {
			Get();
			IdentCont();
		} else if (la.kind == 24) {
			Get();
			Expr();
		} else SynErr(58);
	}

	void StrucStmt() {
		if (la.kind == 17) {
			Get();
			Expect(11);
			Expr();
			Expect(12);
			Expect(15);
			Stmts();
			Expect(16);
		} else if (la.kind == 18) {
			Get();
			Expect(11);
			Stmt();
			Expect(13);
			Expr();
			Expect(13);
			Stmt();
			Expect(12);
			Expect(15);
			Stmts();
			Expect(16);
		} else if (la.kind == 19) {
			Get();
			Expect(11);
			HalfDecl();
			Expect(20);
			CallOrID();
			Expect(12);
			Expect(15);
			Stmts();
			Expect(16);
		} else if (la.kind == 21) {
			Get();
			Expect(11);
			Expr();
			Expect(12);
			Expect(15);
			Stmts();
			Expect(16);
			while (la.kind == 22) {
				Get();
				Expect(11);
				Expr();
				Expect(12);
				Expect(15);
				Stmts();
				Expect(16);
			}
			if (la.kind == 23) {
				Get();
				Expect(15);
				Stmts();
				Expect(16);
			}
		} else SynErr(59);
	}

	void Expr() {
		ExprOR();
	}

	void CallOrID() {
		Expect(1);
		if (la.kind == 11) {
			Get();
			Args();
			Expect(12);
		}
	}

	void FullDecl() {
		Type();
		if (la.kind == 1) {
			Get();
			if (la.kind == 26) {
				Assign();
			}
		} else if (la.kind == 15) {
			Get();
			VtxDecls();
			Expect(16);
		} else if (la.kind == 11) {
			VtxDecl();
		} else SynErr(60);
	}

	void IdentCont() {
		if (la.kind == 26) {
			Assign();
		} else if (la.kind == 11) {
			Get();
			Args();
			Expect(12);
			if (la.kind == 25) {
				Member();
			}
		} else if (la.kind == 27 || la.kind == 28 || la.kind == 29) {
			EdgeOpr();
			EdgeOneOrMore();
		} else if (la.kind == 25) {
			Member();
		} else SynErr(61);
	}

	void Args() {
		if (StartOf(8)) {
			Expr();
			while (WeakSeparator(13,8,9) ) {
				Expr();
			}
		}
	}

	void Member() {
		ExpectWeak(25, 10);
		Expect(1);
		IdentCont();
	}

	void EdgeOpr() {
		if (la.kind == 27) {
			Get();
		} else if (la.kind == 28) {
			Get();
		} else if (la.kind == 29) {
			Get();
		} else SynErr(62);
	}

	void EdgeOneOrMore() {
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 11) {
			EdgeDecl();
		} else if (la.kind == 15) {
			Get();
			EdgeDecls();
			Expect(16);
		} else SynErr(63);
	}

	void VtxDecls() {
		VtxDecl();
		while (WeakSeparator(13,11,12) ) {
			VtxDecl();
		}
	}

	void VtxDecl() {
		Expect(11);
		Expect(1);
		VEParams();
		Expect(12);
	}

	void VEParams() {
		while (WeakSeparator(13,13,5) ) {
			Expect(1);
			Assign();
		}
	}

	void EdgeDecl() {
		Expect(11);
		Expect(1);
		VEParams();
		Expect(12);
	}

	void EdgeDecls() {
		EdgeDecl();
		while (WeakSeparator(13,11,12) ) {
			EdgeDecl();
		}
	}

	void ExprOR() {
		ExprAnd();
		while (la.kind == 30) {
			Get();
			ExprAnd();
		}
	}

	void ExprAnd() {
		ExprEQ();
		while (la.kind == 31) {
			Get();
			ExprEQ();
		}
	}

	void ExprEQ() {
		ExprRel();
		if (la.kind == 32 || la.kind == 33) {
			if (la.kind == 32) {
				Get();
			} else {
				Get();
			}
			ExprRel();
		}
	}

	void ExprRel() {
		ExprPlus();
		if (StartOf(14)) {
			if (la.kind == 34) {
				Get();
			} else if (la.kind == 35) {
				Get();
			} else if (la.kind == 36) {
				Get();
			} else {
				Get();
			}
			ExprPlus();
		}
	}

	void ExprPlus() {
		if (la.kind == 38) {
			Get();
		}
		ExprMult();
		while (la.kind == 38 || la.kind == 39) {
			if (la.kind == 39) {
				Get();
			} else {
				Get();
			}
			ExprMult();
		}
	}

	void ExprMult() {
		ExprNot();
		while (la.kind == 40 || la.kind == 41 || la.kind == 42) {
			if (la.kind == 40) {
				Get();
			} else if (la.kind == 41) {
				Get();
			} else {
				Get();
			}
			ExprNot();
		}
	}

	void ExprNot() {
		if (la.kind == 43) {
			Get();
		}
		Factor();
	}

	void Factor() {
		if (la.kind == 11) {
			Get();
			Expr();
			Expect(12);
		} else if (StartOf(15)) {
			Const();
		} else if (la.kind == 1) {
			CallOrID();
		} else SynErr(64);
	}

	void Const() {
		if (la.kind == 2) {
			Get();
		} else if (la.kind == 3) {
			Get();
		} else if (la.kind == 5) {
			Get();
		} else if (la.kind == 6) {
			Get();
		} else if (la.kind == 4) {
			Get();
		} else SynErr(65);
	}

	void SingleType() {
		if (la.kind == 48) {
			Get();
		} else if (la.kind == 49) {
			Get();
		} else if (la.kind == 50) {
			Get();
		} else if (la.kind == 9) {
			Get();
		} else if (la.kind == 10) {
			Get();
		} else SynErr(66);
	}

	void CollecType() {
		if (la.kind == 44) {
			Get();
			ExpectWeak(34, 10);
			SingleType();
			ExpectWeak(35, 16);
		} else if (la.kind == 45) {
			Get();
			ExpectWeak(34, 10);
			SingleType();
			ExpectWeak(35, 16);
		} else if (la.kind == 46) {
			Get();
			ExpectWeak(34, 10);
			SingleType();
			ExpectWeak(35, 16);
		} else if (la.kind == 47) {
			Get();
			ExpectWeak(34, 10);
			SingleType();
			ExpectWeak(35, 16);
		} else SynErr(67);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		MAGIA();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,T,x,x, x,x,x,T, x,T,T,x, x,x,T,x, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{T,T,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x},
		{x,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{T,T,x,x, x,x,x,T, x,T,T,x, x,x,T,x, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{T,T,x,x, x,x,x,T, x,T,T,T, x,x,T,T, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
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
			case 14: s = "\"func\" expected"; break;
			case 15: s = "\"{\" expected"; break;
			case 16: s = "\"}\" expected"; break;
			case 17: s = "\"while\" expected"; break;
			case 18: s = "\"for\" expected"; break;
			case 19: s = "\"foreach\" expected"; break;
			case 20: s = "\"in\" expected"; break;
			case 21: s = "\"if\" expected"; break;
			case 22: s = "\"elseif\" expected"; break;
			case 23: s = "\"else\" expected"; break;
			case 24: s = "\"return\" expected"; break;
			case 25: s = "\".\" expected"; break;
			case 26: s = "\"=\" expected"; break;
			case 27: s = "\"<-\" expected"; break;
			case 28: s = "\"--\" expected"; break;
			case 29: s = "\"->\" expected"; break;
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
			case 44: s = "\"list\" expected"; break;
			case 45: s = "\"set\" expected"; break;
			case 46: s = "\"queue\" expected"; break;
			case 47: s = "\"stack\" expected"; break;
			case 48: s = "\"number\" expected"; break;
			case 49: s = "\"bool\" expected"; break;
			case 50: s = "\"text\" expected"; break;
			case 51: s = "??? expected"; break;
			case 52: s = "this symbol not expected in MAGIA"; break;
			case 53: s = "invalid Head"; break;
			case 54: s = "this symbol not expected in Stmts"; break;
			case 55: s = "this symbol not expected in FuncDecl"; break;
			case 56: s = "invalid Type"; break;
			case 57: s = "invalid Assign"; break;
			case 58: s = "invalid Stmt"; break;
			case 59: s = "invalid StrucStmt"; break;
			case 60: s = "invalid FullDecl"; break;
			case 61: s = "invalid IdentCont"; break;
			case 62: s = "invalid EdgeOpr"; break;
			case 63: s = "invalid EdgeOneOrMore"; break;
			case 64: s = "invalid Factor"; break;
			case 65: s = "invalid Const"; break;
			case 66: s = "invalid SingleType"; break;
			case 67: s = "invalid CollecType"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}