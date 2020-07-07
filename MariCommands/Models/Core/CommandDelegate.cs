using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// A delegate that can proccess a command request.
    /// </summary>
    /// <param name="context">The current command context.</param>
    /// <returns>A <see cref="Task" /> representing an asynchronous operation</returns>
    public delegate Task CommandDelegate(CommandContext context);
}