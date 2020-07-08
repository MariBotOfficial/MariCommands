using System;
using System.Collections.Generic;
using System.Reflection;

namespace MariCommands.Builder
{
    /// <summary>
    /// A configurer for Add Modules.
    /// </summary>
    public interface IModuleConfigurer
    {
        /// <summary>
        /// General config for <see cref="ICommandService" />
        /// </summary>
        ICommandServiceOptions Options { get; }

        /// <summary>
        /// Search all modules in your project and add them to modules dependency.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        IReadOnlyCollection<IModule> AddModules(Assembly assembly);

        /// <summary>
        /// Add this module to dependency.
        /// </summary>
        /// <param name="type">Any module type.</param>
        /// <typeparam name="T">Any module type.</typeparam>
        IModule AddModule<T>(T type)
            where T : IModule;

        /// <summary>
        /// Add this module type to dependency.
        /// </summary>
        /// <typeparam name="T">Any module type.</typeparam>
        IModule AddModule<T>()
            where T : IModule;

        /// <summary>
        /// Add this module type to dependency.
        /// </summary>
        /// <param name="type">Any module type.</param>
        IModule AddModule(Type type);
    }
}