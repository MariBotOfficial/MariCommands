using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public class VoidTaskExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(VoidTaskCommand);
            var method = type.GetMethod(nameof(VoidTaskCommand.ExecuteAsync));

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);

            var continueWithMethod = methodCall.Type.GetContinueWithMethod();

            Expression<Func<Task, IResult>> continueWithBody = (task) => new SuccessResult();

            var continueWithExpression = Expression.Call(methodCall, continueWithMethod, continueWithBody);

            var lambda = Expression.Lambda<Callback>(continueWithExpression, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return (instance as VoidTaskCommand).ExecuteAsync()
                    .ContinueWith<IResult>((task) =>
                    {
                        return new SuccessResult();
                    });
            };

            var result1 = await callback1(new VoidTaskCommand(), new object[0]);
            var result2 = await callback2(new VoidTaskCommand(), new object[0]);
        }
    }


    public class VoidTaskCommand
    {
        public Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}