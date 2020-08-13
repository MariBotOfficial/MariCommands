using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public class ResultTaskExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(ResultTaskCommand);
            var method = type.GetMethod(nameof(ResultTaskCommand.ExecuteAsync));
            var asyncResultType = method.ReturnType.GetGenericArguments().FirstOrDefault();

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);

            var convertMethod = typeof(ExpressionHelper).GetMethod(nameof(ExpressionHelper.ChangeResultAsync)).MakeGenericMethod(asyncResultType);

            var convertExpression = Expression.Call(convertMethod, new Expression[] { methodCall });

            var lambda = Expression.Lambda<Callback>(convertExpression, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return (instance as ResultTaskCommand).ExecuteAsync()
                        .ChangeResultAsync();
            };

            var result1 = await callback1(new ResultTaskCommand(), new object[0]);
            var result2 = await callback2(new ResultTaskCommand(), new object[0]);
        }
    }

    public class ResultTaskCommand
    {
        public Task<SuccessResult> ExecuteAsync()
        {
            return Task.FromResult(new SuccessResult());
        }
    }
}