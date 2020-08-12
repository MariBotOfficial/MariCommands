using System;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands
{
    /// <summary>
    /// Defines a precondition for this command or module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        /// <summary>
        /// Asynchronously execute this precondition.
        /// </summary>
        /// <param name="context">The current command execution context.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with an <see cref="IPreconditionResult" />.</returns>
        public abstract Task<IPreconditionResult> ExecuteAsync(CommandContext context);
    }
}