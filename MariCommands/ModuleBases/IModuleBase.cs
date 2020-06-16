using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// When implemented will mark the object as a <see cref="IModule" />.
    /// </summary>
    /// <typeparam name="T">The <see cref="CommandContext" /> this module will use.</typeparam>
    public interface IModuleBase<T>
        where T : CommandContext
    {
        /// <summary>
        /// The command context in this module.
        /// </summary>
        T Context { get; }

        /// <summary>
        /// An asynchronous operation that will be executed before the command execution.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task OnCommandExecutingAsync();

        /// <summary>
        /// An asynchronous operation that will be executed after the command execution.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task OnCommandExecutedAsync();
    }
}