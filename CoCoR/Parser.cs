
using System;
using P4_Project.AST;
using P4_Project.AST.Commands;
using P4_Project.AST.Commands.Stmts;
using P4_Project.AST.Commands.Stmts.Decls;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Values;

namespace P4_Project.Compiler.SyntaxAnalysis {



public class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _NUMBER = 2;
	public const int _TEXT = 3;
	public const int _NONE = 4;
	public const int _TRUE = 5;
	public const int _FALSE = 6;
	public const int _RPAREN = 7;
	public const int maxT = 51;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

	public MAGIA mainNode;



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
		mainNode = null; Block mainBlock = new Block(); 
		while (la.kind == 8) {
			while (!(la.kind == 0 || la.kind == 8)) {SynErr(52); Get();}
			Head(out HeadNode headNode);
			mainBlock.Add(headNode); 
		}
		Stmts(ref mainBlock);
		while (la.kind == 14) {
			FuncDecl(out FuncDeclNode funcNode);
			mainBlock.Add(funcNode); 
		}
		mainNode = new MAGIA(mainBlock); 
	}

	void Head(out HeadNode headNode) {
		headNode = null; 
		Expect(8);
		if (la.kind == 9) {
			Get();
			headNode = new HeadNode(HeadNode.VERTEX); 
		} else if (la.kind == 10) {
			Get();
			headNode = new HeadNode(HeadNode.EDGE);   
		} else SynErr(53);
		ExpectWeak(11, 1);
		AttrDecls(ref headNode);
		ExpectWeak(7, 2);
		Expect(12);
	}

	void Stmts(ref Block block) {
		while (!(StartOf(3))) {SynErr(54); Get();}
		while (StartOf(4)) {
			if (StartOf(5)) {
				FullDecl(ref block);
			} else if (la.kind == 1 || la.kind == 24) {
				Stmt(out StmtNode stmtNode);
				block.Add(stmtNode); 
			} else {
				StrucStmt(out StmtNode stmtNode);
				block.Add(stmtNode); 
			}
		}
	}

	void FuncDecl(out FuncDeclNode funcNode) {
		funcNode = null; string funcName = ""; Block paramBlock = new Block(); Block stmtBlock = new Block(); 
		while (!(la.kind == 0 || la.kind == 14)) {SynErr(55); Get();}
		Expect(14);
		Expect(1);
		funcName = t.val; 
		Expect(11);
		if (la.kind != _RPAREN) {
			FuncParams(ref paramBlock);
		}
		Expect(7);
		while (!(la.kind == 0 || la.kind == 15)) {SynErr(56); Get();}
		Expect(15);
		Stmts(ref stmtBlock);
		while (!(la.kind == 0 || la.kind == 16)) {SynErr(57); Get();}
		Expect(16);
		funcNode = new FuncDeclNode(funcName, paramBlock, stmtBlock); 
	}

	void AttrDecls(ref HeadNode headNode) {
		VarDeclNode attrDecl; 
		AttrDecl(out attrDecl);
		headNode?.AddAttr(attrDecl); 
		while (WeakSeparator(13,5,6) ) {
			AttrDecl(out attrDecl);
			headNode?.AddAttr(attrDecl); 
		}
	}

	void AttrDecl(out VarDeclNode attrDecl) {
		attrDecl = null; string name = ""; ExprNode val = null; 
		Type(out int typ);
		Expect(1);
		name = t.val; 
		if (la.kind == 26) {
			Assign(out val);
		}
		attrDecl = new VarDeclNode(typ, name, val); 
	}

	void Type(out int type) {
		type = 0; 
		if (StartOf(7)) {
			SingleType(out type);
		} else if (StartOf(8)) {
			CollecType(out type);
		} else SynErr(58);
	}

	void Assign(out ExprNode expr) {
		expr = null; 
		Expect(26);
		if (StartOf(9)) {
			Expr(out expr);
		} else if (la.kind == 15) {
			Get();
			Args(out CollecConst collec);
			expr = collec; 
			Expect(16);
		} else SynErr(59);
	}

	void FuncParams(ref Block paramBlock) {
		int typ = 0; 
		Type(out typ);
		Expect(1);
		paramBlock.Add(new VarDeclNode(typ, t.val, null)); 
		while (la.kind == 13) {
			while (!(la.kind == 0 || la.kind == 13)) {SynErr(60); Get();}
			Get();
			Type(out typ);
			Expect(1);
			paramBlock.Add(new VarDeclNode(typ, t.val, null)); 
		}
	}

	void FullDecl(ref Block block) {
		Type(out int typ);
		if (la.kind == 1) {
			Get();
			string name = t.val; ExprNode expr = null; 
			if (la.kind == 26) {
				Assign(out expr);
			}
			block.Add(new VarDeclNode(typ, name, expr)); 
		} else if (la.kind == 15) {
			Get();
			VtxDecls(ref block);
			Expect(16);
		} else if (la.kind == 11) {
			VtxDecl(out VEDeclNode veDecl);
			block.Add(veDecl); 
		} else SynErr(61);
	}

	void Stmt(out StmtNode stmtNode) {
		stmtNode = null; 
		if (la.kind == 1) {
			CallOrID(out IdentNode i);
			while (la.kind == 25) {
				Member();
			}
			IdentCont();
		} else if (la.kind == 24) {
			Get();
			Expr(out ExprNode expr);
		} else SynErr(62);
	}

	void StrucStmt(out StmtNode s) {
		s = null; 
		if (la.kind == 17) {
			StmtWhile(out s);
		} else if (la.kind == 18) {
			StmtFor(out s);
		} else if (la.kind == 19) {
			StmtForeach(out s);
		} else if (la.kind == 21) {
			StmtIf(out s);
		} else SynErr(63);
	}

	void StmtWhile(out StmtNode w) {
		w = null; Block b = new Block(); 
		Expect(17);
		Expect(11);
		Expr(out ExprNode e);
		Expect(7);
		Expect(15);
		Stmts(ref b);
		w = new WhileNode(e, b); 
		Expect(16);
	}

	void StmtFor(out StmtNode f) {
		f = null; StmtNode init = null; ExprNode e = null; StmtNode iter = null; Block b = new Block(); 
		Expect(18);
		Expect(11);
		Stmt(out StmtNode s1);
		init = s1; 
		Expect(13);
		Expr(out ExprNode e1);
		e = e1;  
		Expect(13);
		Stmt(out StmtNode s2);
		iter = s2; 
		Expect(7);
		Expect(15);
		Stmts(ref b);
		f = new ForNode(init, e, iter, b); 
		Expect(16);
	}

	void StmtForeach(out StmtNode f) {
		f = null; Block b = new Block(); VarDeclNode v = null; 
		Expect(19);
		Expect(11);
		Type(out int typ);
		Expect(1);
		v = new VarDeclNode(typ, t.val, null); 
		Expect(20);
		Expr(out ExprNode e1);
		
		Expect(7);
		Expect(15);
		Stmts(ref b);
		f = new ForeachNode(v, e1, b); 
		Expect(16);
	}

	void StmtIf(out StmtNode i) {
		i = null; ExprNode e = null; Block b = new Block(); IfNode j = null; IfNode k = null; 
		Expect(21);
		Expect(11);
		Expr(out ExprNode ie1);
		e = ie1; 
		Expect(7);
		Expect(15);
		Stmts(ref b);
		i = new IfNode(e, b); j = (IfNode)i; 
		Expect(16);
		while (la.kind == 22) {
			Get();
			Expect(11);
			Expr(out ExprNode ie2);
			e = ie2; b = new Block(); 
			Expect(7);
			Expect(15);
			Stmts(ref b);
			k = new IfNode(e, b); j.SetElse(k); j = k;  
			Expect(16);
		}
		if (la.kind == 23) {
			Get();
			b = new Block(); 
			Expect(15);
			Stmts(ref b);
			k = new IfNode(new BoolConst(true), b); j.SetElse(k); 
			Expect(16);
		}
	}

	void Expr(out ExprNode e) {
		e = null; 
		ExprOR(out e);
	}

	void CallOrID(out IdentNode i) {
		Identifier(out i);
		if (la.kind == 11) {
			Get();
			Args(out CollecConst collec);
			Expect(7);
		}
	}

	void Member() {
		ExpectWeak(25, 1);
		CallOrID(out IdentNode i);
	}

	void IdentCont() {
		if (la.kind == 26) {
			Assign(out ExprNode assign);
		} else if (la.kind == 27 || la.kind == 28 || la.kind == 29) {
			EdgeOpr(out int i);
			EdgeOneOrMore();
		} else SynErr(64);
	}

	void EdgeOpr(out int op) {
		op = 0;					 
		if (la.kind == 27) {
			Get();
			op = Operators.LEFTARR;	 
		} else if (la.kind == 28) {
			Get();
			op = Operators.NONARR;	 
		} else if (la.kind == 29) {
			Get();
			op = Operators.RIGHTARR;	 
		} else SynErr(65);
	}

	void EdgeOneOrMore() {
		if (la.kind == 1) {
			Identifier(out IdentNode identNode);
		} else if (la.kind == 11) {
			EdgeDecl();
		} else if (la.kind == 15) {
			Get();
			EdgeDecls();
			Expect(16);
		} else SynErr(66);
	}

	void VtxDecls(ref Block b) {
		VtxDecl(out VEDeclNode v1);
		b.Add(v1); 
		while (WeakSeparator(13,10,11) ) {
			VtxDecl(out VEDeclNode v2);
			b.Add(v2); 
		}
	}

	void VtxDecl(out VEDeclNode v) {
		v = null; 
		Expect(11);
		Expect(1);
		v = new VEDeclNode(Types.vertex, t.val); 
		VEParams(out AssignNode a);
		v.AddAttr(a); 
		Expect(7);
	}

	void Args(out CollecConst collec) {
		collec = new CollecConst(); ExprNode expr; 
		if (StartOf(9)) {
			Expr(out expr);
			collec.Add(expr); 
			while (WeakSeparator(13,9,12) ) {
				Expr(out expr);
				collec.Add(expr); 
			}
		}
	}

	void VEParams(out AssignNode assign) {
		assign = null; 
		while (WeakSeparator(13,13,6) ) {
			Identifier(out IdentNode identNode);
			Assign(out ExprNode assign);
		}
	}

	void Identifier(out IdentNode identNode) {
		identNode = null; 
		Expect(1);
		identNode = new IdentNode(); 
	}

	void EdgeDecl() {
		Expect(11);
		Expect(1);
		VEParams(out AssignNode assign);
		Expect(7);
	}

	void EdgeDecls() {
		EdgeDecl();
		while (WeakSeparator(13,10,11) ) {
			EdgeDecl();
		}
	}

	void ExprOR(out ExprNode e) {
		e = null; int op = 0; 
		ExprAnd(out ExprNode e1);
		e = e1; 
		while (la.kind == 30) {
			Get();
			op  = Operators.OR; 
			ExprAnd(out ExprNode e2);
			e = new BinExprNode(e, op, e2); 
		}
	}

	void ExprAnd(out ExprNode e) {
		e = null; int op = 0; 
		ExprEQ(out ExprNode e1);
		e = e1; 
		while (la.kind == 31) {
			Get();
			op = Operators.EQ; 
			ExprEQ(out ExprNode e2);
			e = new BinExprNode(e, op, e2); 
		}
	}

	void ExprEQ(out ExprNode e) {
		e = null; int op = 0; 
		ExprRel(out ExprNode e1);
		e = e1; 
		if (la.kind == 32 || la.kind == 33) {
			if (la.kind == 32) {
				Get();
				op = Operators.EQ; 
			} else {
				Get();
				op = Operators.NEQ; 
			}
			ExprRel(out ExprNode e2);
			e = new BinExprNode(e, op, e2); 
		}
	}

	void ExprRel(out ExprNode e) {
		e = null; int op = 0; 
		ExprPlus(out ExprNode e1);
		e = e1; 
		if (StartOf(14)) {
			if (la.kind == 34) {
				Get();
				op = Operators.LESS; 
			} else if (la.kind == 35) {
				Get();
				op = Operators.GREATER; 
			} else if (la.kind == 36) {
				Get();
				op = Operators.LESSEQ; 
			} else {
				Get();
				op = Operators.GREATEQ; 
			}
			ExprPlus(out ExprNode e2);
			e = new BinExprNode(e, op, e2); 
		}
	}

	void ExprPlus(out ExprNode e) {
		e = null; bool b = false; int op = 0; 
		if (la.kind == 38) {
			Get();
			b = true; 
		}
		ExprMult(out ExprNode e1);
		if(b) e = new UnaExprNode(Operators.UMIN, e1); else e = e1; 
		while (la.kind == 38 || la.kind == 39) {
			if (la.kind == 39) {
				Get();
				op = Operators.PLUS; 
			} else {
				Get();
				op = Operators.BIMIN; 
			}
			ExprMult(out ExprNode e2);
			e = new BinExprNode(e, op, e2); 
		}
	}

	void ExprMult(out ExprNode e) {
		e = null; int op = 0;
		ExprNot(out ExprNode e1);
		e = e1; 
		while (la.kind == 40 || la.kind == 41 || la.kind == 42) {
			if (la.kind == 40) {
				Get();
				op = Operators.MULT; 
			} else if (la.kind == 41) {
				Get();
				op = Operators.DIV; 
			} else {
				Get();
				op = Operators.MOD; 
			}
			ExprNot(out ExprNode e2);
			e = new BinExprNode(e, op, e2); 
		}
	}

	void ExprNot(out ExprNode e) {
		e = null; bool b = false;
		if (la.kind == 43) {
			Get();
			b = true; 
		}
		Factor(out e);
		if(b) e = new UnaExprNode(Operators.NOT, e); 
	}

	void Factor(out ExprNode e) {
		e = null; 
		if (StartOf(15)) {
			Const(out e);
		} else if (la.kind == 1 || la.kind == 11) {
			if (la.kind == 11) {
				Get();
				Expr(out e);
				Expect(7);
			} else {
				CallOrID(out IdentNode i);
			}
			while (la.kind == 25) {
				Member();
			}
		} else SynErr(67);
	}

	void Const(out ExprNode e) {
		e = null; 
		if (la.kind == 2) {
			Get();
			e = new NumConst(Convert.ToInt32(t.val));    
		} else if (la.kind == 3) {
			Get();
			e = new TextConst(t.val);                    
		} else if (la.kind == 5) {
			Get();
			e = new BoolConst(Convert.ToBoolean(t.val)); 
		} else if (la.kind == 6) {
			Get();
			e = new BoolConst(Convert.ToBoolean(t.val)); 
		} else if (la.kind == 4) {
			Get();
			e = new NoneConst();                         
		} else SynErr(68);
	}

	void SingleType(out int type) {
		type = 0; 
		if (la.kind == 48) {
			Get();
			type = Types.number;
		} else if (la.kind == 49) {
			Get();
			type = Types.boolean;
		} else if (la.kind == 50) {
			Get();
			type = Types.text; 
		} else if (la.kind == 9) {
			Get();
			type = Types.vertex; 
		} else if (la.kind == 10) {
			Get();
			type = Types.edge; 
		} else SynErr(69);
	}

	void CollecType(out int type) {
		type = 0; int subType = 0; 
		if (la.kind == 44) {
			Get();
			ExpectWeak(34, 1);
			SingleType(out subType);
			ExpectWeak(35, 16);
			type = Types.list + subType; 
		} else if (la.kind == 45) {
			Get();
			ExpectWeak(34, 1);
			SingleType(out subType);
			ExpectWeak(35, 16);
			type = Types.set + subType; 
		} else if (la.kind == 46) {
			Get();
			ExpectWeak(34, 1);
			SingleType(out subType);
			ExpectWeak(35, 16);
			type = Types.queue + subType; 
		} else if (la.kind == 47) {
			Get();
			ExpectWeak(34, 1);
			SingleType(out subType);
			ExpectWeak(35, 16);
			type = Types.stack + subType; 
		} else SynErr(70);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		MAGIA();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,T,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{T,T,x,x, x,x,x,x, T,T,T,x, x,T,T,T, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{T,T,x,x, x,x,x,x, T,T,T,x, T,T,T,T, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{T,T,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x},
		{x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x},
		{x,T,T,T, T,T,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{T,T,x,x, x,x,x,x, T,T,T,T, x,T,T,T, T,T,T,T, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,x, x}

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
			case 56: s = "this symbol not expected in FuncDecl"; break;
			case 57: s = "this symbol not expected in FuncDecl"; break;
			case 58: s = "invalid Type"; break;
			case 59: s = "invalid Assign"; break;
			case 60: s = "this symbol not expected in FuncParams"; break;
			case 61: s = "invalid FullDecl"; break;
			case 62: s = "invalid Stmt"; break;
			case 63: s = "invalid StrucStmt"; break;
			case 64: s = "invalid IdentCont"; break;
			case 65: s = "invalid EdgeOpr"; break;
			case 66: s = "invalid EdgeOneOrMore"; break;
			case 67: s = "invalid Factor"; break;
			case 68: s = "invalid Const"; break;
			case 69: s = "invalid SingleType"; break;
			case 70: s = "invalid CollecType"; break;

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