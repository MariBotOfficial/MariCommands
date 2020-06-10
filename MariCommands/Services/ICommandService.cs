using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Execute a command with the params in the same string.
        /// </summary>
        /// <param name="command">The command name to be executed.</param>
        /// <returns>An <see cref="IResult" /> of this command execution.</returns>
        Task<IResult> ExecuteAsync(string command);

        /// <summary>
        /// Execute a command with the params.
        /// </summary>
        /// <param name="command">The command name to be executed.</param>
        /// <param name="args">The args to be parsed for this command.</param>
        /// <returns>An <see cref="IResult" /> of this command execution.</returns>
        Task<IResult> ExecuteAsync(string command, string[] args);

        /// <summary>
        /// Execute a command with the params.
        /// </summary>
        /// <param name="command">The command name to be executed.</param>
        /// <param name="args">The args to be parsed for this command.</param>
        /// <returns>An <see cref="IResult" /> of this command execution.</returns>
        Task<IResult> ExecuteAsync(string command, IEnumerable<string> args)
            => ExecuteAsync(command, args.ToArray());

        /// <summary>
        /// Execute a command with the params.
        /// </summary>
        /// <param name="command">The command name to be executed.</param>
        /// <param name="args">The args to be parsed for this command.</param>
        /// <returns>An <see cref="IResult" /> of this command execution.</returns>
        Task<IResult> ExecuteAsync(string command, object[] args)
            => ExecuteAsync(command, args.Select(a => a.ToString()).ToArray());

        /// <summary>
        /// Execute a command with the params.
        /// </summary>
        /// <param name="command">The command name to be executed.</param>
        /// <param name="args">The args to be parsed for this command.</param>
        /// <returns>An <see cref="IResult" /> of this command execution.</returns>
        Task<IResult> ExecuteAsync(string command, IEnumerable<object> args)
            => ExecuteAsync(command, args.ToArray());
    }
}