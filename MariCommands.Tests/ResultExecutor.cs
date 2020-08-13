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

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);

            var taskMethod = ExpressionHelper.GetTaskResultMethod();

            var taskResultExpression = Expression.Call(taskMethod, new Expression[] { methodCall, });

            var lambda = Expression.Lambda<Callback>(taskResultExpression, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return Task.FromResult((instance as ResultCommand).Execute());
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