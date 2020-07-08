using System.Threading.Tasks;

namespace MariCommands.Middlewares
{
    /// <summary>
    /// Represents a middleware that can proccess a command request.
    /// </summary>
    public interface ICommandMiddleware
    {
        /// <summary>
        /// Invoke the current Middleware
        /// </summary>
        /// <param name="context">The current command context.</param>
        /// <param name="next">A delegate that will pass te pipeline to the next middleware.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task InvokeAsync(CommandContext context, CommandDelegate next);
    }
}