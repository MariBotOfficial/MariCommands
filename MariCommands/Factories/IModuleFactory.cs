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
        /// <param name="parent">The parent module (can be <see langword="null" />).</param>
        /// <param name="type">The submodule type to build.</param>
        /// <returns>A builded submodule.</returns>
        IModuleBuilder BuildModule(IModuleBuilder parent, Type type);

        /// <summary>
        /// Verify if the type is a module.
        /// </summary>
        /// <param name="type">The module type.</param>
        /// <returns><see langword="true" /> if the specified type is a module.</returns>
        bool IsModule(Type type);

        /// <summary>
        /// Verify if the type is a submodule.
        /// </summary>
        /// <param name="type">The submodule type.</param>
        /// <returns><see langword="true" /> if the specified type is a submodule.</returns>
        bool IsSubModule(Type type);
    }
}