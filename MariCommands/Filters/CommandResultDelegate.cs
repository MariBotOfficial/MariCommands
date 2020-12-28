using System.Threading.Tasks;

namespace MariCommands.Filters
{
    /// <summary>
    /// A delegate that can proccess a result of a command request.
    /// </summary>
    /// <param name="context">The current command result context.</param>
    /// <returns>A <see cref="Task" /> representing an asynchronous operation</returns>
    public delegate Task CommandResultDelegate(CommandResultContext context);
}