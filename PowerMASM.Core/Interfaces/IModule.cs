using PowerMASM.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for a modular component, including metadata and lifecycle management methods.
    /// </summary>
    /// <remarks>Implementations of this interface provide identifying information such as name, version,
    /// author, and description, and support controlled initialization and shutdown. This interface is intended for use
    /// With the PowerMASM Module runtime. Thread safety and error handling behaviors may vary depending on the implementation.</remarks>
    public interface IModule
    {
        /// <summary>
        /// Gets the name associated with the current instance.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the version identifier for the current instance.
        /// </summary>
        string Version { get; }
        string Author { get; }
        string Description { get; }

        // Lifecycle methods
        
        /// <summary>
        /// Initializes the component and prepares it for use.
        /// </summary>
        /// <returns>An integer value indicating the result of the initialization. A value of 0 typically indicates success;
        /// other values may indicate specific error conditions.</returns>
        int Initialize();
        /// <summary>
        /// Initiates a graceful shutdown of the service, releasing resources and stopping ongoing operations as
        /// appropriate.
        /// </summary>
        /// <remarks>After calling this method, the service may become unavailable for further requests.
        /// Ensure that all necessary operations are completed before invoking shutdown.</remarks>
        void Shutdown();
    }
}
