using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.Executors
{
    internal sealed class ResultExecutor : ICommandExecutor
    {
        private readonly CommandCallback _callback;

        private ResultExecutor(CommandCallback callback)
        {
            _callback = callback;
        }

        public Task<IResult> ExecuteAsync(object module, object[] args)
            => _callback(module, args);

        public static ResultExecutor Create(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            var (body, instanceParameter, argsParameter) = ExecutorUtils.GetBasicExpressions(moduleBuilder, commandBuilder);

            var callback = ExecutorUtils.CreateCallbackForTaskResult(body, instanceParameter, argsParameter);

            return new ResultExecutor(callback);
        }
    }
}