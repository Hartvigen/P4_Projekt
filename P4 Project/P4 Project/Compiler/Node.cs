using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.Compiler
{
    abstract class Node
    {
    }

    class MagiaNode : Node
    {
        
    }

    class HeaderNode : Node
    {
        List<VarDeclNode> EdgeAttr = new List<VarDeclNode>();
        List<VarDeclNode> VtxAttr = new List<VarDeclNode>();
    }

    class VarDeclNode : Node
    {
        Type type;
        string identity;
        
    }

    class StmtsNode : Node
    {

    }

    class FuncDecl : Node
    {

    }
}
