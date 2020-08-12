using MariCommands.Executors;

namespace MariCommands.Providers
{
    /// <summary>
    /// An interface that can provider an <see cref="ICommandExecutor" /> to a specific command.
    /// </summary>
    public interface ICommandExecutorProvider
    {
        /// <summary>
        /// Get a command executor for the specified command.
        /// </summary>
        /// <param name="moduleBuilder">The moduleBuilder for this command.</param>
        /// <param name="commandBuilder">The commandBuilder for this command.</param>
        /// <returns>A command executor for the specified command.</returns>
        ICommandExecutor GetCommandExecutor(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder);
    }
}