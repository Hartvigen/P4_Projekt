using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4_Project.AST.Stmts.Decls
{
    /// <summary>
    /// The "VEDeclNode" is the base class for vertex and edge declarations. 
    /// This class exist because the declaration of vertices and edges have some common aspects.
    /// </summary>
    abstract class VEDeclNode : DeclNode
    {
        Block attributes = new Block();

        public VEDeclNode() { }

        protected VEDeclNode(string _symbolName) : base(_symbolName)
        {
        }

        public void AddAttr(AssignNode assign)
        {
            attributes.Add(assign);
        }
    }
}