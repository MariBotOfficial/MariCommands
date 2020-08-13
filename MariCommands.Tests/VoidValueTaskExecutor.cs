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

            var valueTask1Exp = Expression.Variable(method.ReturnType, "valueTask1");

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);
            var methodAssign = Expression.Assign(valueTask1Exp, methodCall);

            var asTaskMethod = method.ReturnType.GetMethod(nameof(ValueTask.AsTask));
            var task1Exp = Expression.Variable(asTaskMethod.ReturnType, "task1");

            var asTaskExp = Expression.Call(methodAssign, asTaskMethod);
            var asTaskAssign = Expression.Assign(task1Exp, asTaskExp);

            var continueWithMethod = asTaskMethod.ReturnType.GetContinueWithMethod();

            Expression<Func<Task, IResult>> continueWithBody = (task) => new SuccessResult();

            var continueWithExpression = Expression.Call(asTaskAssign, continueWithMethod, continueWithBody);

            var body = Expression.Block(
                new ParameterExpression[] { valueTask1Exp, task1Exp },
                continueWithExpression
            );

            var lambda = Expression.Lambda<Callback>(body, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                var valueTask1 = (instance as VoidValueTaskCommand).ExecuteAsync();

                var task1 = valueTask1.AsTask();

                return task1.ContinueWith<IResult>((task) =>
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