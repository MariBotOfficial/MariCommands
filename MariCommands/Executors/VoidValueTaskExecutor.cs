using System.Threading.Tasks;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.Executors
{
    internal sealed class VoidValueTaskExecutor : ICommandExecutor
    {
        private readonly CommandCallback _callback;

        private VoidValueTaskExecutor(CommandCallback callback)
        {
            _callback = callback;
        }

        public Task<IResult> ExecuteAsync(object module, object[] args)
            => _callback(module, args);

        public static VoidValueTaskExecutor Create(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            var (body, instanceParameter, argsParameter) = ExecutorUtils.GetBasicExpressions(moduleBuilder, commandBuilder);

            body = ExecutorUtils.AddAsTaskExpression(body);

            body = ExecutorUtils.AddContinueWithExpression(body);

            var callback = ExecutorUtils.CreateCallback(body, instanceParameter, argsParameter);

            return new VoidValueTaskExecutor(callback);
        }
    }
}