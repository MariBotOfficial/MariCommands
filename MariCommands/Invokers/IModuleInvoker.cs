using System;
using System.Threading.Tasks;

namespace MariCommands.Invokers
{
    /// <summary>
    /// An interface that can instanciate modules.
    /// </summary>
    public interface IModuleInvoker
    {
        /// <summary>
        /// Create a new instance of this module.
        /// </summary>
        /// <param name="provider">The service container to use.</param>
        /// <returns>A new instance of this module.</returns>
        object CreateInstance(IServiceProvider provider);
    }
}