using System;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// A service that can cache modules.
    /// </summary>
    public interface IModuleCache
    {
        /// <summary>
        /// Asynchronously adds a module to the cache and returns if success.
        /// </summary>
        /// <param name="module">The module to be added.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a boolean.</returns>
        Task<bool> AddModuleAsync(IModule module);

        /// <summary>
        /// Asynchronously removes a module to the cache and returns if success.
        /// </summary>
        /// <param name="module">The module to be removed.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a boolean.</returns>
        Task<bool> RemoveModuleAsync(IModule module);

        /// <summary>
        /// Asynchronously search for commands with the specified input.
        /// </summary>
        /// <param name="input">The input to search commands.</param>
        /// <returns>A <see cref="ValueTask" /> representing an asynchronous operation with
        /// a collection of matched commands.</returns>
        ValueTask<ICommandMatch> SearchCommandsAsync(string input);
    }
}