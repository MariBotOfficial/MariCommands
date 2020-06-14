using System;
using System.Reflection;

namespace MariCommands
{
    /// <summary>
    /// A service that can build and create commands.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Build a comand for the specified method.
        /// </summary>
        /// <param name="type">The module type to build commands</param>
        /// <param name="methodInfo">The real method info.</param>
        /// <returns>A builded command.</returns>
        ICommandBuilder BuildCommand(Type type, MethodInfo methodInfo);

        /// <summary>
        /// Verify if the method is a command.
        /// </summary>
        /// <param name="type">The module type.</param>
        /// <param name="methodInfo">The real method info to be verified.</param>
        /// <returns>If this is a valid command.</returns>
        bool IsCommand(Type type, MethodInfo methodInfo);
    }
}