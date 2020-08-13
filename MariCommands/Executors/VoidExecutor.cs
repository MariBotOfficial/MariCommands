using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.Executors
{
    internal sealed class VoidExecutor : ICommandExecutor
    {
        private readonly CommandCallback _callback;

        private VoidExecutor(CommandCallback callback)
        {
            _callback = callback;
        }

        public Task<IResult> ExecuteAsync(object module, object[] args)
            => _callback(module, args);

        public static VoidExecutor Create(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            var (body, instanceParameter, argsParameter) = ExecutorUtils.GetBasicExpressions(moduleBuilder, commandBuilder);

            var resultExpression = Expression.Constant(Task.FromResult(new SuccessResult() as IResult));

            body = Expression.Block(body, resultExpression);

            var callback = ExecutorUtils.CreateCallback(body, instanceParameter, argsParameter);

            return new VoidExecutor(callback);
        }
    }
}