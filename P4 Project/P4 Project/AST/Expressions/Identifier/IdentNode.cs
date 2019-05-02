namespace P4_Project.AST.Expressions.Identifier
{
    /// <summary>
    /// This node represents an identifier
    /// </summary>
    public abstract class IdentNode : ExprNode
    {
        /// <summary>
        /// The Ident string is used as the key for looking up objects in the symbol table.
        /// </summary>
        public string Ident { get;}
        public IdentNode Source { get; set; }
        protected IdentNode(string ident)
        {
            Ident = ident;
        }
    }
}
