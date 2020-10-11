using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// When implemented will mark the object as a <see cref="IModule" />.
    /// </summary>
    public interface IModuleBase
    {
        /// <summary>
        /// An asynchronous operation that will be executed after the command execution.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task OnCommandExecutedAsync();

        /// <summary>
        /// An asynchronous operation that will be executed before the command execution.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task OnCommandExecutingAsync();

        /// <summary>
        /// Sets the context for this module.
        /// </summary>
        /// <param name="context">The context to be set for this module.</param>
        void SetContext(CommandContext context);
    }
}