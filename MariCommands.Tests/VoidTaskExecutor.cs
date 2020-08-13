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

            var task1Exp = Expression.Variable(typeof(Task), "task1");

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);
            var methodAssign = Expression.Assign(task1Exp, methodCall);

            var continueWithMethod = methodCall.Type.GetContinueWithMethod();

            Expression<Func<Task, IResult>> continueWithBody = (task) => new SuccessResult();

            var continueWithExpression = Expression.Call(methodAssign, continueWithMethod, continueWithBody);

            var body = Expression.Block(
                new ParameterExpression[] { task1Exp },
                continueWithExpression
            );

            var lambda = Expression.Lambda<Callback>(body, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                var task1 = (instance as VoidTaskCommand).ExecuteAsync();

                var task2 = task1.ContinueWith<IResult>((task) =>
                {
                    return new SuccessResult();
                });

                return task2;
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