using System.Collections.Generic;

namespace P4_Project.AST
{
    /// <summary>
    /// Class is used in most of the compiler in some capacity.
    /// It has the responsebility to be able to reflect any type in the entire langauge.
    /// </summary>
    public class BaseType
    {
        //Generel Type name
        public string name;

        //Function Type
        public string returntype = "none";
        public List<BaseType> parameterTypes;

        //CollectionTypes
        public BaseType collectionType;
        public BaseType singleType;

        //Used to do a little reachability analysis
        public bool reached;

        /// <summary>
        /// Constructor used to create a simple type
        /// </summary>
        /// <param name="type">The type name eg. number, vertex ..</param>
        public BaseType(string type) {
            this.name = type;
        }

        /// <summary>
        /// Constructor used for creating a function BaseType
        /// </summary>
        /// <param name="returntype">The Return type of the function</param>
        /// <param name="parameterTypes">A List of all the parameters the function needs can be empty but not null</param>
        public BaseType(BaseType returntype, List<BaseType> parameterTypes)
        {
            this.name = "func";
            if(returntype != null)
                this.returntype = returntype.name;
            this.parameterTypes = parameterTypes;
        }
        
        /// <summary>
        /// Constructor used for creating collection types
        /// </summary>
        /// <param name="singleType">The subtype the collection is holding</param>
        /// <param name="collectionType">The collection type eg. Set, Stack ..</param>
        public BaseType(BaseType singleType, BaseType collectionType)
        {
            this.name = "collec";
            this.collectionType = collectionType;
            this.singleType = singleType;
        }
    }
}
