using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.Executors
{
    /// <summary>
    /// An interface that can execute a command.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Verify if this executor can execute this command.
        /// </summary>
        /// <param name="command">The commmand definition.</param>
        /// <returns><see langword="true" /> if this executor can execute this command.</returns>
        bool CanExecute(ICommand command);

        /// <summary>
        /// Execute the command with the specified arguments.
        /// </summary>
        /// <param name="module">The module instance.</param>
        /// <param name="args">The arguments for the command.</param>
        /// <returns>A <see cref="ValueTask" /> representing an asynchronous operation
        /// with an <see cref="IResult" />.</returns>
        ValueTask<IResult> ExecuteAsync(object module, object[] args);
    }
}