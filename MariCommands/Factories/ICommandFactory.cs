using System;
using System.Reflection;

namespace MariCommands.Factories
{
    /// <summary>
    /// A service that can build and create commands.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Build a comand for the specified method.
        /// </summary>
        /// <param name="module">The module builder of this command.</param>
        /// <param name="methodInfo">The real method info.</param>
        /// <returns>A builded command.</returns>
        ICommandBuilder BuildCommand(IModuleBuilder module, MethodInfo methodInfo);

        /// <summary>
        /// Verify if the method is a command.
        /// </summary>
        /// <param name="module">The module builder of this command.</param>
        /// <param name="methodInfo">The real method info to be verified.</param>
        /// <returns>If this is a valid command.</returns>
        bool IsCommand(IModuleBuilder module, MethodInfo methodInfo);
    }
}