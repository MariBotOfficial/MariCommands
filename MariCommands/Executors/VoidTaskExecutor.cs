using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.Executors
{
    internal sealed class VoidTaskExecutor : ICommandExecutor
    {
        private readonly CommandCallback _callback;

        private VoidTaskExecutor(CommandCallback callback)
        {
            _callback = callback;
        }

        public Task<IResult> ExecuteAsync(object module, object[] args)
            => _callback(module, args);

        public static VoidTaskExecutor Create(IModuleBuilder moduleBuilder, ICommandBuilder commandBuilder)
        {
            var (body, instanceParameter, argsParameter) = ExecutorUtils.GetBasicExpressions(moduleBuilder, commandBuilder);

            body = ExecutorUtils.AddContinueWithExpression(body);

            var callback = ExecutorUtils.CreateCallback(body, instanceParameter, argsParameter);

            return new VoidTaskExecutor(callback);
        }
    }
}