using System;
using System.Threading.Tasks;

namespace MariCommands.Factories
{
    /// <summary>
    /// A service that can build and create modules.
    /// </summary>
    public interface IModuleFactory
    {
        /// <summary>
        /// Asynchronously build a module.
        /// </summary>
        /// <param name="type">The module type to build.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a builded module.</returns>
        Task<IModuleBuilder> BuildModuleAsync(Type type);

        /// <summary>
        /// Asynchronously verify if the type is a module.
        /// </summary>
        /// <param name="type">The module type.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a boolean.</returns>
        Task<bool> IsModuleAsync(Type type);
    }
}