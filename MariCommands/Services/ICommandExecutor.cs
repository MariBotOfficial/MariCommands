using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// A service that can execute commands.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Execute a command with the params in the same string.
        /// </summary>
        /// <param name="command">The command name to be executed.</param>
        /// <returns>An <see cref="IResult" /> of this command execution.</returns>
        Task<IResult> ExecuteAsync(string command);

        /// <summary>
        /// Execute a command with the params.
        /// </summary>
        /// <param name="command">The command name to be executed.</param>
        /// <param name="args">The args to be parsed for this command.</param>
        /// <returns>An <see cref="IResult" /> of this command execution.</returns>
        Task<IResult> ExecuteAsync(string command, string[] args);
    }
}