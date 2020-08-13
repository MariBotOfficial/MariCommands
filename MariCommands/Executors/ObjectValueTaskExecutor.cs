using System.Threading.Tasks;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.Executors
{
    internal sealed class ObjectValueTaskExecutor : ICommandExecutor
    {
        private readonly CommandCallback _callback;

        private ObjectValueTaskExecutor(CommandCallback callback)
        {
            _callback = callback;
        }

        public Task<IResult> ExecuteAsync(object module, object[] args)
            => _callback(module, args);

        public static ObjectValueTaskExecutor Create(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            var (body, instanceParameter, argsParameter) = ExecutorUtils.GetBasicExpressions(moduleBuilder, commandBuilder);

            body = ExecutorUtils.AddAsTaskExpression(body);

            body = ExecutorUtils.ChangeForObjectTaskResultExpression(body, commandBuilder.AsyncResultType);

            var callback = ExecutorUtils.CreateCallback(body, instanceParameter, argsParameter);

            return new ObjectValueTaskExecutor(callback);
        }
    }
}