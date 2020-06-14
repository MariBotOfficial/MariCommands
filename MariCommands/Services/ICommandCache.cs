using System;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// A service that can cache modules.
    /// </summary>
    public interface ICommandCache
    {
        /// <summary>
        /// Asynchronously adds a command to the cache and returns if success.
        /// </summary>
        /// <param name="command">The command to be added.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a boolean.</returns>
        Task<bool> AddCommandAsync(ICommand command);

        /// <summary>
        /// Asynchronously removes a command to the cache and returns if success.
        /// </summary>
        /// <param name="command">The command to be removed.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a boolean.</returns>
        Task<bool> RemoveCommandAsync(ICommand command);

        /// <summary>
        /// Asynchronously search for commands with the specified input.
        /// </summary>
        /// <param name="input">The input to search commands.</param>
        /// <param name="comparison">The comparison to be used.</param>
        /// <returns>A <see cref="ValueTask" /> representing an asynchronous operation with
        /// a collection of matched commands.</returns>
        ValueTask<ICommandMatch> SearchCommandsAsync(string input, StringComparison? comparison = null);
    }
}