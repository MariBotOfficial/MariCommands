using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace MariCommands.Tests
{
    public class ObjectTaskExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(ObjectTaskCommand);
            var method = type.GetMethod(nameof(ObjectTaskCommand.ExecuteAsync));
            var asyncResultType = method.ReturnType.GetGenericArguments().FirstOrDefault();

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var task1Exp = Expression.Variable(method.ReturnType, "task1");

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);
            var methodAssign = Expression.Assign(task1Exp, methodCall);

            var convertMethod = typeof(ExpressionHelper).GetMethod(nameof(ExpressionHelper.ChangeObjectResultAsync)).MakeGenericMethod(asyncResultType);

            var continueWithExpression = Expression.Call(convertMethod, new Expression[] { methodAssign });

            var body = Expression.Block(
                new ParameterExpression[] { task1Exp },
                continueWithExpression
            );

            var lambda = Expression.Lambda<Callback>(body, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                var task1 = (instance as ObjectTaskCommand).ExecuteAsync();

                var task2 = task1.ChangeObjectResultAsync();

                return task2;
            };

            var result1 = await callback1(new ObjectTaskCommand(), new object[0]);
            var result2 = await callback2(new ObjectTaskCommand(), new object[0]);
        }
    }

    public class ObjectTaskCommand
    {
        public Task<object> ExecuteAsync()
        {
            return Task.FromResult(new object());
        }
    }
}