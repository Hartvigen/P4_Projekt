using System;
using System.Collections.Generic;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts.Decls
{
    public class MultiDecl : StmtNode
    {
        public List<DeclNode> Decls { get; private set; }


        public MultiDecl()
        {
            Decls = new List<DeclNode>();
        }


        public override object Accept(Visitor vi, object o)
        {
            vi.Visit(this, null);

            return null;
        }

        public void AddDecl(DeclNode decl)
        {
            Decls.Add(decl);
        }
    }
}
