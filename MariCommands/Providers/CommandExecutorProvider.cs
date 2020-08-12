using MariCommands.Executors;

namespace MariCommands.Providers
{
    internal sealed class CommandExecutorProvider : ICommandExecutorProvider
    {
        public ICommandExecutor GetCommandExecutor(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            throw new System.NotImplementedException();
        }
    }
}