using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.Executors
{
    internal sealed class ObjectExecutor : ICommandExecutor
    {
        private readonly CommandCallback _callback;

        private ObjectExecutor(CommandCallback callback)
        {
            _callback = callback;
        }

        public Task<IResult> ExecuteAsync(object module, object[] args)
            => _callback(module, args);

        public static ObjectExecutor Create(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            var (body, instanceParameter, argsParameter) = ExecutorUtils.GetBasicExpressions(moduleBuilder, commandBuilder);

            var objectResultCtor = ExecutorUtils.GetObjectResultCtor();

            body = Expression.New(objectResultCtor, body);
            body = Expression.Convert(body, typeof(IResult));

            var callback = ExecutorUtils.CreateCallbackForTaskResult(body, instanceParameter, argsParameter);

            return new ObjectExecutor(callback);
        }
    }
}