using System.Threading.Tasks;

namespace MariCommands.Filters
{
    /// <summary>
    /// A delegate that can proccess a command exception request.
    /// </summary>
    /// <param name="context">The current command exception context.</param>
    /// <returns>A <see cref="Task" /> representing an asynchronous operation</returns>
    public delegate Task CommandExceptionDelegate(CommandExceptionContext context);
}