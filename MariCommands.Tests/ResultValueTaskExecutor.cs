using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public class ResultValueTaskExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(ResultValueTaskCommand);
            var method = type.GetMethod(nameof(ObjectTaskCommand.ExecuteAsync));
            var asyncResultType = method.ReturnType.GetGenericArguments().FirstOrDefault();

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);

            var asTaskMethod = method.ReturnType.GetMethod(nameof(ValueTask.AsTask));
            var asTaskExp = Expression.Call(methodCall, asTaskMethod);

            var convertMethod = typeof(ExpressionHelper).GetMethod(nameof(ExpressionHelper.ChangeResultAsync)).MakeGenericMethod(asyncResultType);

            var convertExpression = Expression.Call(convertMethod, new Expression[] { asTaskExp });

            var lambda = Expression.Lambda<Callback>(convertExpression, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return (instance as ResultValueTaskCommand).ExecuteAsync()
                            .AsTask()
                            .ChangeResultAsync();
            };

            var result1 = await callback1(new ResultValueTaskCommand(), new object[0]);
            var result2 = await callback2(new ResultValueTaskCommand(), new object[0]);
        }
    }

    public class ResultValueTaskCommand
    {
        public ValueTask<SuccessResult> ExecuteAsync()
        {
            return new ValueTask<SuccessResult>(new SuccessResult());
        }
    }
}