using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.Interfaces
{
    public class IModuleFunctionAttribute: Attribute
    {
        /// <summary>
        /// Gets the name of the module associated with this instance.
        /// </summary>
        public string ModuleName { get; }
        /// <summary>
        /// Gets the name of the function associated with this instance.
        /// </summary>
        public string FunctionName { get; }
        /// <summary>
        /// Gets the type information for the function represented by this member.
        /// </summary>
        /// The type information is used to get the function arguments via reflection for calling into "Native" Code
        private Type Function { get; }
        public IModuleFunctionAttribute(string moduleName, string functionName)
        {
            ModuleName = moduleName;
            FunctionName = functionName;
        }
    }
}
