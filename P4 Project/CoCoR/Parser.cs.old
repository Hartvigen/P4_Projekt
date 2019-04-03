
using System;
using P4_Project.AST;
using P4_Project.AST.Stmts;
using P4_Project.AST.Stmts.Decls;
using P4_Project.AST.Expressions;
using P4_Project.AST.Expressions.Identifier;
using P4_Project.AST.Expressions.Values;
using P4_Project.SymbolTable;

namespace P4_Project.Compiler.SyntaxAnalysis {



public class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _NUMBER = 2;
	public const int _TEXT = 3;
	public const int _NONE = 4;
	public const int _TRUE = 5;
	public const int _FALSE = 6;
	public const int maxT = 53;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

	public MAGIA mainNode;

public SymbolTable tab;
/*public CodeGenerator gen;*/
    
  
/*--------------------------------------------------------------------------*/
/* The following section contains the token specification of MAGIA.*/


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
		while (la.kind == 7) {
			while (!(la.kind == 0 || la.kind == 7)) {SynErr(54); Get();}
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
		Expect(7);
		if (la.kind == 8) {
			Get();
			headNode = new HeadNode(HeadNode.VERTEX); 
		} else if (la.kind == 9) {
			Get();
			headNode = new HeadNode(HeadNode.EDGE);   
		} else SynErr(55);
		ExpectWeak(10, 1);
		AttrDecls(ref headNode);
		ExpectWeak(11, 2);
		Expect(12);
	}

	void Stmts(ref Block block) {
		while (!(StartOf(3))) {SynErr(56); Get();}
		while (StartOf(4)) {
			if (StartOf(5)) {
				FullDecl(ref block);
			} else if (StartOf(6)) {
				Stmt(out StmtNode stmtNode);
				block.Add(stmtNode); 
			} else {
				StrucStmt(out StmtNode stmtNode);
				block.Add(stmtNode); 
			}
		}
	}

	void FuncDecl(out FuncDeclNode funcNode) {
		Obj obj; funcNode = null; string funcName = ""; Block paramBlock = new Block(); Block stmtBlock = new Block(); 
		while (!(la.kind == 0 || la.kind == 14)) {SynErr(57); Get();}
		Expect(14);
		Expect(1);
		funcName = t.val; obj = tab.NewObj(t.val, t.kind, null);
		Expect(10);
		if (la.val != ")") {
			FuncParams(ref paramBlock);
		}
		Expect(11);
		while (!(la.kind == 0 || la.kind == 15)) {SynErr(58); Get();}
		Expect(15);
		Stmts(ref stmtBlock);
		while (!(la.kind == 0 || la.kind == 16)) {SynErr(59); Get();}
		Expect(16);
		funcNode = new FuncDeclNode(funcName, paramBlock, stmtBlock); 
	}

	void AttrDecls(ref HeadNode headNode) {
		VarDeclNode attrDecl; 
		AttrDecl(out attrDecl);
		headNode?.AddAttr(attrDecl); 
		while (WeakSeparator(13,5,7) ) {
			AttrDecl(out attrDecl);
			headNode?.AddAttr(attrDecl); 
		}
	}

	void AttrDecl(out VarDeclNode attrDecl) {
		attrDecl = null; string name = ""; ExprNode val = null; 
		Type(out int typ);
		Expect(1);
		name = t.val; 
		if (la.kind == 28) {
			Assign(out val);
		}
		attrDecl = new VarDeclNode(typ, name, val); 
	}

	void Type(out int type) {
		type = 0; 
		if (StartOf(8)) {
			SingleType(out type);
		} else if (StartOf(9)) {
			CollecType(out type);
		} else SynErr(60);
	}

	void Assign(out ExprNode expr) {
		expr = null; 
		Expect(28);
		if (StartOf(10)) {
			Expr(out expr);
		} else if (la.kind == 15) {
			Get();
			Args(out CollecConst collec);
			expr = collec; 
			Expect(16);
		} else SynErr(61);
	}

	void FuncParams(ref Block paramBlock) {
		int typ = 0; Obj obj; 
		Type(out typ);
		obj = tab.NewObj(t.val, t.kind, typ); 
		Expect(1);
		paramBlock.Add(new VarDeclNode(typ, t.val, null)); 
		while (la.kind == 13) {
			while (!(la.kind == 0 || la.kind == 13)) {SynErr(62); Get();}
			Get();
			Type(out typ);
			Expect(1);
			paramBlock.Add(new VarDeclNode(typ, t.val, null)); 
		}
	}

	void FullDecl(ref Block block) {
		Obj obj; 
		Type(out int typ);
		obj = tab.NewObj(t.val, t.kind, typ); 
		if (la.kind == 1) {
			Get();
			string name = t.val; ExprNode expr = null; 
			if (la.kind == 28) {
				Assign(out expr);
			}
			block.Add(new VarDeclNode(typ, name, expr)); 
		} else if (la.kind == 15) {
			Get();
			VtxDecls(ref block);
			Expect(16);
		} else if (la.kind == 10) {
			VtxDecl(out VertexDeclNode VertexDecl);
			block.Add(VertexDecl); 
		} else SynErr(63);
	}

	void Stmt(out StmtNode stmtNode) {
		stmtNode = null; 
		if (la.kind == 1) {
			CallOrID(out IdentNode i);
			stmtNode = new LoneCallNode(i); 
			while (la.kind == 27) {
				Member(i, out MemberNode member);
				i = member; stmtNode = new LoneCallNode(i); 
			}
			if (StartOf(11)) {
				IdentCont(i, out StmtNode stmt);
				stmtNode = stmt; 
			}
		} else if (la.kind == 24) {
			Get();
			Expr(out ExprNode expr);
			stmtNode = new ReturnNode(expr); 
		} else if (la.kind == 25) {
			Get();
			stmtNode = new BreakNode(); 
		} else if (la.kind == 26) {
			Get();
			stmtNode = new ContinueNode(); 
		} else SynErr(64);
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
		} else SynErr(65);
	}

	void StmtWhile(out StmtNode w) {
		w = null; Block b = new Block(); 
		Expect(17);
		Expect(10);
		Expr(out ExprNode e);
		Expect(11);
		Expect(15);
		Stmts(ref b);
		w = new WhileNode(e, b); 
		Expect(16);
	}

	void StmtFor(out StmtNode f) {
		f = null; StmtNode init = null; ExprNode e = null; StmtNode iter = null; Block b = new Block(); 
		Expect(18);
		Expect(10);
		Stmt(out StmtNode s1);
		init = s1; 
		Expect(13);
		Expr(out ExprNode e1);
		e = e1;  
		Expect(13);
		Stmt(out StmtNode s2);
		iter = s2; 
		Expect(11);
		Expect(15);
		Stmts(ref b);
		f = new ForNode(init, e, iter, b); 
		Expect(16);
	}

	void StmtForeach(out StmtNode f) {
		f = null; Block b = new Block(); VarDeclNode v = null; 
		Expect(19);
		Expect(10);
		Type(out int typ);
		Expect(1);
		v = new VarDeclNode(typ, t.val, null); 
		Expect(20);
		Expr(out ExprNode e1);
		Expect(11);
		Expect(15);
		Stmts(ref b);
		f = new ForeachNode(v, e1, b); 
		Expect(16);
	}

	void StmtIf(out StmtNode i) {
		i = null; ExprNode e = null; Block b = new Block(); IfNode j = null; IfNode k = null; 
		Expect(21);
		Expect(10);
		Expr(out ExprNode ie1);
		e = ie1; 
		Expect(11);
		Expect(15);
		Stmts(ref b);
		i = new IfNode(e, b); j = (IfNode)i; 
		Expect(16);
		while (la.kind == 22) {
			Get();
			Expect(10);
			Expr(out ExprNode ie2);
			e = ie2; b = new Block(); 
			Expect(11);
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
			k = new IfNode(null, b); j.SetElse(k); 
			Expect(16);
		}
	}

	void Expr(out ExprNode e) {
		e = null; 
		ExprOR(out e);
	}

	void CallOrID(out IdentNode i) {
		i = null; 
		Identifier(out VarNode varNode);
		i = varNode; 
		if (la.kind == 10) {
			Get();
			Args(out CollecConst collec);
			i = new CallNode(i.identifier, collec); 
			Expect(11);
		}
	}

	void Member(ExprNode source, out MemberNode mem) {
		mem = null; 
		ExpectWeak(27, 1);
		CallOrID(out IdentNode i);
		mem = new MemberNode(source, i); 
	}

	void IdentCont(IdentNode i, out StmtNode s) {
		s = null; Block b = new Block(); 
		if (la.kind == 28) {
			Assign(out ExprNode expr);
			s = new AssignNode(i, expr); 
		} else if (la.kind == 29 || la.kind == 30 || la.kind == 31) {
			EdgeOpr(out int op);
			EdgeOneOrMore(i, op, ref b);
			s = b; 
		} else SynErr(66);
	}

	void EdgeOpr(out int op) {
		op = 0;                  
		if (la.kind == 29) {
			Get();
			op = Operators.LEFTARR;  
		} else if (la.kind == 30) {
			Get();
			op = Operators.NONARR;   
		} else if (la.kind == 31) {
			Get();
			op = Operators.RIGHTARR; 
		} else SynErr(67);
	}

	void EdgeOneOrMore(IdentNode left, int op, ref Block b) {
		if (la.kind == 1) {
			Identifier(out VarNode varNode);
			b.Add(new EdgeDeclNode(left, varNode, op)); 
		} else if (la.kind == 10) {
			EdgeDecl(left, op, out EdgeDeclNode edge);
			b.Add(edge); 
		} else if (la.kind == 15) {
			Get();
			EdgeDecls(left, op, ref b);
			Expect(16);
		} else SynErr(68);
	}

	void VtxDecls(ref Block b) {
		VtxDecl(out VertexDeclNode v1);
		b.Add(v1); 
		while (WeakSeparator(13,12,13) ) {
			VtxDecl(out VertexDeclNode v2);
			b.Add(v2); 
		}
	}

	void VtxDecl(out VertexDeclNode v) {
		v = null; Obj obj; 
		Expect(10);
		Expect(1);
		v = new VertexDeclNode(Types.vertex, t.val); obj = tab.NewObj(t.val, t.kind, null); 
		VEParams(v);
		Expect(11);
	}

	void Args(out CollecConst collec) {
		collec = new CollecConst(); ExprNode expr; 
		if (StartOf(10)) {
			Expr(out expr);
			collec.Add(expr); 
			while (WeakSeparator(13,10,14) ) {
				Expr(out expr);
				collec.Add(expr); 
			}
		}
	}

	void VEParams(VEDeclNode ve) {
		while (WeakSeparator(13,15,7) ) {
			Identifier(out VarNode varNode);
			Assign(out ExprNode expr);
			ve.AddAttr(new AssignNode(varNode, expr)); 
		}
	}

	void Identifier(out VarNode varNode) {
		varNode = null; 
		Expect(1);
		varNode = new VarNode(t.val); 
	}

	void EdgeDecl(IdentNode left, int op, out EdgeDeclNode edge) {
		edge = null; Obj obj; 
		Expect(10);
		Identifier(out VarNode right);
		edge = new EdgeDeclNode(left, right, op); obj = tab.NewObj(t.val, t.kind, null); 
		VEParams(edge);
		Expect(11);
	}

	void EdgeDecls(IdentNode left, int op, ref Block b) {
		EdgeDecl(left, op, out EdgeDeclNode e1);
		b.Add(e1); 
		while (WeakSeparator(13,12,13) ) {
			EdgeDecl(left, op, out EdgeDeclNode e2);
			b.Add(e2); 
		}
	}

	void ExprOR(out ExprNode e) {
		e = null; int op = 0; 
		ExprAnd(out ExprNode e1);
		e = e1; 
		while (la.kind == 32) {
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
		while (la.kind == 33) {
			Get();
			op = Operators.AND; 
			ExprEQ(out ExprNode e2);
			e = new BinExprNode(e, op, e2); 
		}
	}

	void ExprEQ(out ExprNode e) {
		e = null; int op = 0; 
		ExprRel(out ExprNode e1);
		e = e1; 
		if (la.kind == 34 || la.kind == 35) {
			if (la.kind == 34) {
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
		if (StartOf(16)) {
			if (la.kind == 36) {
				Get();
				op = Operators.LESS; 
			} else if (la.kind == 37) {
				Get();
				op = Operators.GREATER; 
			} else if (la.kind == 38) {
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
		if (la.kind == 40) {
			Get();
			b = true; 
		}
		ExprMult(out ExprNode e1);
		if(b) e = new UnaExprNode(Operators.UMIN, e1); else e = e1; 
		while (la.kind == 40 || la.kind == 41) {
			if (la.kind == 41) {
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
		while (la.kind == 42 || la.kind == 43 || la.kind == 44) {
			if (la.kind == 42) {
				Get();
				op = Operators.MULT; 
			} else if (la.kind == 43) {
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
		if (la.kind == 45) {
			Get();
			b = true; 
		}
		Factor(out e);
		if(b) e = new UnaExprNode(Operators.NOT, e); 
	}

	void Factor(out ExprNode e) {
		e = null; 
		if (StartOf(17)) {
			Const(out e);
		} else if (la.kind == 1 || la.kind == 10) {
			if (la.kind == 10) {
				Get();
				Expr(out e);
				e.inParentheses = true; 
				Expect(11);
			} else {
				CallOrID(out IdentNode ident);
				e = ident; 
			}
			while (la.kind == 27) {
				Member(e, out MemberNode member);
				e = member; 
			}
		} else SynErr(69);
	}

	void Const(out ExprNode e) {
		e = null; 
		if (la.kind == 2) {
			Get();
			e = new NumConst(Convert.ToDouble(t.val, System.Globalization.CultureInfo.InvariantCulture));    
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
		} else SynErr(70);
	}

	void SingleType(out int type) {
		type = 0; 
		if (la.kind == 50) {
			Get();
			type = Types.number; 
		} else if (la.kind == 51) {
			Get();
			type = Types.boolean; 
		} else if (la.kind == 52) {
			Get();
			type = Types.text; 
		} else if (la.kind == 8) {
			Get();
			type = Types.vertex; 
		} else if (la.kind == 9) {
			Get();
			type = Types.edge; 
		} else SynErr(71);
	}

	void CollecType(out int type) {
		type = 0; int subType = 0; 
		if (la.kind == 46) {
			Get();
			ExpectWeak(36, 1);
			SingleType(out subType);
			ExpectWeak(37, 18);
			type = Types.list + subType; 
		} else if (la.kind == 47) {
			Get();
			ExpectWeak(36, 1);
			SingleType(out subType);
			ExpectWeak(37, 18);
			type = Types.set + subType; 
		} else if (la.kind == 48) {
			Get();
			ExpectWeak(36, 1);
			SingleType(out subType);
			ExpectWeak(37, 18);
			type = Types.queue + subType; 
		} else if (la.kind == 49) {
			Get();
			ExpectWeak(36, 1);
			SingleType(out subType);
			ExpectWeak(37, 18);
			type = Types.stack + subType; 
		} else SynErr(72);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		MAGIA();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_T,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _x,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
		{_T,_T,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _x,_T,_T,_T, _T,_T,_T,_T, _x,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
		{_T,_T,_x,_x, _x,_x,_x,_T, _T,_T,_x,_x, _T,_T,_T,_T, _T,_T,_T,_T, _x,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
		{_T,_T,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _x,_x,_T,_x, _T,_T,_T,_T, _x,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _x,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x},
		{_x,_T,_T,_T, _T,_T,_T,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_x,_x,_T,_T, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x},
		{_T,_T,_x,_x, _x,_x,_x,_T, _T,_T,_T,_x, _x,_T,_T,_T, _T,_T,_T,_T, _x,_T,_x,_x, _T,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_T,_T, _T,_x,_x}

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
			case 8: s = "\"vertex\" expected"; break;
			case 9: s = "\"edge\" expected"; break;
			case 10: s = "\"(\" expected"; break;
			case 11: s = "\")\" expected"; break;
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
			case 25: s = "\"break\" expected"; break;
			case 26: s = "\"continue\" expected"; break;
			case 27: s = "\".\" expected"; break;
			case 28: s = "\"=\" expected"; break;
			case 29: s = "\"<-\" expected"; break;
			case 30: s = "\"--\" expected"; break;
			case 31: s = "\"->\" expected"; break;
			case 32: s = "\"||\" expected"; break;
			case 33: s = "\"&&\" expected"; break;
			case 34: s = "\"==\" expected"; break;
			case 35: s = "\"!=\" expected"; break;
			case 36: s = "\"<\" expected"; break;
			case 37: s = "\">\" expected"; break;
			case 38: s = "\"<=\" expected"; break;
			case 39: s = "\">=\" expected"; break;
			case 40: s = "\"-\" expected"; break;
			case 41: s = "\"+\" expected"; break;
			case 42: s = "\"*\" expected"; break;
			case 43: s = "\"/\" expected"; break;
			case 44: s = "\"%\" expected"; break;
			case 45: s = "\"!\" expected"; break;
			case 46: s = "\"list\" expected"; break;
			case 47: s = "\"set\" expected"; break;
			case 48: s = "\"queue\" expected"; break;
			case 49: s = "\"stack\" expected"; break;
			case 50: s = "\"number\" expected"; break;
			case 51: s = "\"boolean\" expected"; break;
			case 52: s = "\"text\" expected"; break;
			case 53: s = "??? expected"; break;
			case 54: s = "this symbol not expected in MAGIA"; break;
			case 55: s = "invalid Head"; break;
			case 56: s = "this symbol not expected in Stmts"; break;
			case 57: s = "this symbol not expected in FuncDecl"; break;
			case 58: s = "this symbol not expected in FuncDecl"; break;
			case 59: s = "this symbol not expected in FuncDecl"; break;
			case 60: s = "invalid Type"; break;
			case 61: s = "invalid Assign"; break;
			case 62: s = "this symbol not expected in FuncParams"; break;
			case 63: s = "invalid FullDecl"; break;
			case 64: s = "invalid Stmt"; break;
			case 65: s = "invalid StrucStmt"; break;
			case 66: s = "invalid IdentCont"; break;
			case 67: s = "invalid EdgeOpr"; break;
			case 68: s = "invalid EdgeOneOrMore"; break;
			case 69: s = "invalid Factor"; break;
			case 70: s = "invalid Const"; break;
			case 71: s = "invalid SingleType"; break;
			case 72: s = "invalid CollecType"; break;

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