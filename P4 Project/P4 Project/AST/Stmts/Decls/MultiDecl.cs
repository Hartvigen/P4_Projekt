using System.Collections.Generic;
using P4_Project.Visitors;

namespace P4_Project.AST.Stmts.Decls
{
    public class MultiDecl : StmtNode
    {
        public List<DeclNode> Decls { get; }

        public MultiDecl()
        {
            Decls = new List<DeclNode>();
        }


        public override object Accept(Visitor vi)
        {
            return vi.Visit(this);
        }

        public void AddDecl(DeclNode decl)
        {
            Decls.Add(decl);
        }
    }
}
