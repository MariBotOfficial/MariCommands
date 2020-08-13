using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace MariCommands.Tests
{
    public class ObjectValueTaskExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(ObjectValueTaskCommand);
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

            var convertMethod = typeof(ExpressionHelper).GetMethod(nameof(ExpressionHelper.ChangeObjectResultAsync)).MakeGenericMethod(asyncResultType);

            var convertExpression = Expression.Call(convertMethod, new Expression[] { asTaskExp });

            var lambda = Expression.Lambda<Callback>(convertExpression, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return (instance as ObjectValueTaskCommand).ExecuteAsync()
                            .AsTask()
                            .ChangeObjectResultAsync();
            };

            var result1 = await callback1(new ObjectValueTaskCommand(), new object[0]);
            var result2 = await callback2(new ObjectValueTaskCommand(), new object[0]);
        }
    }

    public class ObjectValueTaskCommand
    {
        public ValueTask<object> ExecuteAsync()
        {
            return new ValueTask<object>(new object());
        }
    }
}