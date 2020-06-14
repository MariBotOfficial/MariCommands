using System;

namespace MariCommands
{
    /// <summary>
    /// A service that can build and create modules.
    /// </summary>
    public interface IModuleFactory
    {
        /// <summary>
        /// Build a module.
        /// </summary>
        /// <param name="type">The module type to build.</param>
        /// <returns>A builded module.</returns>
        IModuleBuilder BuildModule(Type type);

        /// <summary>
        /// Verify if the type is a module.
        /// </summary>
        /// <param name="type">The module type.</param>
        /// <returns><see langword="true" /> if the specified type is a module.</returns>
        bool IsModule(Type type);
    }
}