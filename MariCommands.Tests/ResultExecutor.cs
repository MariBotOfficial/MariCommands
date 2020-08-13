using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public class ResultExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(ResultCommand);
            var method = type.GetMethod(nameof(ResultCommand.Execute));

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var resultVarExp = Expression.Variable(typeof(IResult), "result");

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);
            var methodAssign = Expression.Assign(resultVarExp, methodCall);

            var taskMethod = ExpressionHelper.GetTaskResultMethod();

            var taskResultExpression = Expression.Call(taskMethod, new Expression[] { methodAssign, });

            var body = Expression.Block(
                new ParameterExpression[] { resultVarExp },
                taskResultExpression
            );

            var lambda = Expression.Lambda<Callback>(body, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                IResult result = (instance as ResultCommand).Execute();

                return Task.FromResult(result);
            };

            var result1 = await callback1(new ResultCommand(), new object[0]);
            var result2 = await callback2(new ResultCommand(), new object[0]);
        }
    }

    public class ResultCommand
    {
        // can be SuccessResult too
        public IResult Execute()
        {
            return new SuccessResult();
        }
    }
}