using System.CodeDom;
using System.Collections.Generic;

namespace P4_Project.AST
{
    /// <summary>
    /// Class is used in most of the compiler in some capacity.
    /// It has the responsibility to be able to reflect any type in the entire language.
    /// </summary>
    public class BaseType
    {
        //General Type name
        public string name;

        //Function Type
        public readonly BaseType returnType;
        public readonly List<BaseType> parameterTypes;

        //CollectionTypes
        public readonly BaseType collectionType;
        public readonly BaseType singleType;

        //Used to do a little reachable analysis
        public bool reached;

        /// <summary>
        /// Constructor used to create a simple type
        /// </summary>
        /// <param name="type">The type name eg. number, vertex ..</param>
        public BaseType(string type) {
            name = type;
        }

        /// <summary>
        /// Constructor used for creating a function BaseType
        /// </summary>
        /// <param name="returnType">The Return type of the function</param>
        /// <param name="parameterTypes">A List of all the parameters the function needs can be empty but not null</param>
        public BaseType(BaseType returnType, List<BaseType> parameterTypes)
        {
            name = "func";
            this.returnType = returnType;
            this.parameterTypes = parameterTypes;
        }
        
        /// <summary>
        /// Constructor used for creating collection types
        /// </summary>
        /// <param name="singleType">The subtype the collection is holding</param>
        /// <param name="collectionType">The collection type eg. Set, Stack ..</param>
        public BaseType(BaseType singleType, BaseType collectionType)
        {
            name = "collec";
            this.collectionType = collectionType;
            this.singleType = singleType;
        }
    }
}
