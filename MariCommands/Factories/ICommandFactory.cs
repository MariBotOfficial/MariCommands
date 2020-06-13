using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// A service that can build and create commands.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Asynchronously build a comand for the specified method.
        /// </summary>
        /// <param name="type">The module type to build commands</param>
        /// <param name="methodInfo">The real method info.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a builded command.</returns>
        Task<ICommandBuilder> BuildCommandAsync(Type type, MethodInfo methodInfo);

        /// <summary>
        /// Asynchronously verify if thhe method is command.
        /// </summary>
        /// <param name="type">The module type.</param>
        /// <param name="methodInfo">The real method info to be verified.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with a boolean.</returns>
        Task<bool> IsCommandAsync(Type type, MethodInfo methodInfo);
    }
}