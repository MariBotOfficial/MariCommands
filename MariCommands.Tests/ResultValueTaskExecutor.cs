using System;
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
            // var type = typeof(ResultValueTaskCommand);
            // var method = type.GetMethod(nameof(ObjectTaskCommand.ExecuteAsync));

            // var instanceParameter = Expression.Parameter(typeof(object), "instance");
            // var argsParameter = Expression.Parameter(typeof(object[]), "args");

            // var paramInfos = method.GetParameters();
            // var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            // var valueTask1Exp = Expression.Variable(method.ReturnType, "valueTask1");

            // var instanceCast = Expression.Convert(instanceParameter, type);
            // var methodCall = Expression.Call(instanceCast, method, parameters);
            // var methodAssign = Expression.Assign(valueTask1Exp, methodCall);

            // var asTaskMethod = method.ReturnType.GetMethod(nameof(ValueTask.AsTask));
            // var task1Exp = Expression.Variable(asTaskMethod.ReturnType, "task1");

            // var asTaskExp = Expression.Call(methodAssign, asTaskMethod);
            // var asTaskAssign = Expression.Assign(task1Exp, asTaskExp);

            // var continueWithMethod = asTaskMethod.ReturnType.GetContinueWithMethod();

            // Expression<Func<Task, IResult>> continueWithBody = (task) => new SuccessResult();

            // var continueWithExpression = Expression.Call(asTaskAssign, continueWithMethod, continueWithBody);

            // var body = Expression.Block(
            //     new ParameterExpression[] { valueTask1Exp, task1Exp },
            //     continueWithExpression
            // );

            // var lambda = Expression.Lambda<Callback>(body, instanceParameter, argsParameter);
            // var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return (instance as ResultValueTaskCommand).ExecuteAsync()
                            .AsTask()
                            .ChangeResultAsync();
            };

            var result1 = await callback1(new ResultValueTaskCommand(), new object[0]);
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