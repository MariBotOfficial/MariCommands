using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public class VoidValueTaskExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(VoidValueTaskCommand);
            var method = type.GetMethod(nameof(ObjectTaskCommand.ExecuteAsync));

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);

            var asTaskMethod = method.ReturnType.GetMethod(nameof(ValueTask.AsTask));

            var asTaskExp = Expression.Call(methodCall, asTaskMethod);

            var continueWithMethod = asTaskMethod.ReturnType.GetContinueWithMethod();

            Expression<Func<Task, IResult>> continueWithBody = (task) => new SuccessResult();

            var continueWithExpression = Expression.Call(asTaskExp, continueWithMethod, continueWithBody);

            var lambda = Expression.Lambda<Callback>(continueWithExpression, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return (instance as VoidValueTaskCommand).ExecuteAsync()
                        .AsTask()
                        .ContinueWith<IResult>((task) =>
                        {
                            return new SuccessResult();
                        });
            };

            var result1 = await callback1(new VoidValueTaskCommand(), new object[0]);
            var result2 = await callback2(new VoidValueTaskCommand(), new object[0]);
        }
    }

    public class VoidValueTaskCommand
    {
        public ValueTask ExecuteAsync()
        {
            return default;
        }
    }
}