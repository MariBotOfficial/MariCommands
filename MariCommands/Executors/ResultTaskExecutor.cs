using System.Threading.Tasks;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.Executors
{
    internal sealed class ResultTaskExecutor : ICommandExecutor
    {
        private readonly CommandCallback _callback;

        private ResultTaskExecutor(CommandCallback callback)
        {
            _callback = callback;
        }

        public Task<IResult> ExecuteAsync(object module, object[] args)
            => _callback(module, args);

        public static ResultTaskExecutor Create(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            var (body, instanceParameter, argsParameter) = ExecutorUtils.GetBasicExpressions(moduleBuilder, commandBuilder);

            body = ExecutorUtils.ChangeForTaskResultExpression(body, commandBuilder.AsyncResultType);

            var callback = ExecutorUtils.CreateCallback(body, instanceParameter, argsParameter);

            return new ResultTaskExecutor(callback);
        }
    }
}