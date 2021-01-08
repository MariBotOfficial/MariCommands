using System.Threading.Tasks;

namespace MariCommands.Filters
{
    /// <summary>
    /// Marker interface for filters handled in command execution when has an exception.
    /// </summary>
    public interface ICommandExceptionFilter : ICommandFilter
    {
        /// <summary>
        /// Invoke the current exception filter.
        /// </summary>
        /// <param name="context">The current exception context.</param>
        /// <param name="next">A delegate that will pass the pipeline to the next exception filter.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task InvokeAsync(CommandExceptionContext context, CommandExceptionDelegate next);
    }
}