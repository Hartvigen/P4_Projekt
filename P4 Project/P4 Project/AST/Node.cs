using P4_Project.Visitors;

namespace P4_Project.AST
{
    /// <summary>
    /// This abstract class represents, as the name suggests, the basic node from which all inherits.
    /// </summary>
    public abstract class Node
    {
        public abstract void Accept(Visitor vi);
    }
}
