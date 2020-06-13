using System;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// Defines a precondition for this param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ParamPreconditionAttribute : Attribute
    {
        /// <summary>
        /// Asynchronously execute this parameter precondition.
        /// </summary>
        /// <param name="value">The parsed value of this parameter.</param>
        /// <param name="parameter">The parameter that is linked to that attribute.</param>
        /// <param name="context">The current command execution context.</param>
        /// <param name="provider">The current dependencies container.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation
        /// with an <see cref="IPreconditionResult" />.</returns>
        public abstract Task<IPreconditionResult> ExecuteAsync(
            object value,
            IParameter parameter,
            ICommandContext context,
            IServiceProvider provider);
    }
}