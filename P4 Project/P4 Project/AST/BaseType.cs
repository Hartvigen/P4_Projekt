using System;
using System.Collections.Generic;

namespace P4_Project.AST
{
    public class BaseType
    {
        public string name;
        public string returntype = "none";
        public BaseType collectionType;
        public List<BaseType> parameterTypes;
        public BaseType singleType;
        //For all kinds of nodes
        public BaseType(string type) {
            this.name = type;
        }
        //For the FuncDeclNode
        public BaseType(BaseType returntype, List<BaseType> parameterTypes)
        {
            this.name = "func";
            if(returntype != null)
                this.returntype = returntype.name;
            this.parameterTypes = parameterTypes;
        }
        //For the Collections
        public BaseType(BaseType singleType, BaseType collectionType)
        {
            this.name = "collec";
            this.collectionType = collectionType;
            this.singleType = singleType;
        }
        public override string ToString()
        {
            return name;
        }
    }
}
