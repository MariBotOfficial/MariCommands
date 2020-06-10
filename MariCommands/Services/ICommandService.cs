using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// Represents a Command service that can execute commands.
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Search all modules in your project and add them to command dependency.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>An async operation.</returns>
        Task AddModulesAsync(Assembly assembly);

        /// <summary>
        /// Add this module to dependency.
        /// </summary>
        /// <param name="type">Any module type.</param>
        /// <typeparam name="T">Any module type.</typeparam>
        /// <returns>An async operation.</returns>
        Task AddModuleAsync<T>(T type);

        /// <summary>
        /// Add this module type to dependency.
        /// </summary>
        /// <typeparam name="T">Any module type.</typeparam>
        /// <returns>An async operation.</returns>
        Task AddModuleAsync<T>();

        /// <summary>
        /// Add this module type to dependency.
        /// </summary>
        /// <param name="type">Any module type.</param>
        /// <returns>An async operation.</returns>
        Task AddModuleAsync(Type type);
    }
}