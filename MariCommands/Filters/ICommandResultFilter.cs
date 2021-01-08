using System.Threading.Tasks;

namespace MariCommands.Filters
{
    /// <summary>
    /// Marker interface for filters handled in command execution when has a result.
    /// </summary>
    public interface ICommandResultFilter : ICommandFilter
    {
        /// <summary>
        /// Invoke the current result filter.
        /// </summary>
        /// <param name="context">The current result command context.</param>
        /// <param name="next">A delegate that will pass the pipeline to the next result filter.</param>
        /// <returns>A <see cref="Task" /> that represents an asynchronous operation.</returns>
        Task InvokeAsync(CommandResultContext context, CommandResultDelegate next);
    }
}