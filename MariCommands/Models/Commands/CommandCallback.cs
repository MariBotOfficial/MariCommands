using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.Models.Commands
{
    /// <summary>
    /// A builder callback to executors use to execute a command.
    /// </summary>
    /// <param name="module">The module instance.</param>
    /// <param name="args">The arguments of this command.</param>
    /// <returns>A <see cref="Task" /> representing an asynchronous operation
    /// with an <see cref="IResult" />.</returns>
    public delegate Task<IResult> CommandCallback(object module, object[] args);
}