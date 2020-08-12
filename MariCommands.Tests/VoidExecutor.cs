using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public delegate ValueTask<IResult> Callback(object instance, object[] args);

    public class VoidTest
    {
        [Fact]
        public async Task SampleTest1()
        {
            var type = typeof(VoidCommand);
            var method = type.GetMethod(nameof(VoidCommand.Execute));

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = new List<Expression>();

            for (var i = 0; i < paramInfos.Length; i++)
            {
                var param = paramInfos[i];
                var valueParam = Expression.ArrayIndex(argsParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueParam, param.ParameterType);

                parameters.Add(valueCast);
            }

            var instanceCast = Expression.Convert(instanceParameter, type);
            var methodCall = Expression.Call(instanceCast, method, parameters);
            var resultExpression = Expression.Constant(new ValueTask<IResult>(new SuccessResult() as IResult));

            var body = Expression.Block(methodCall, resultExpression);

            var lambda = Expression.Lambda<Callback>(body, instanceParameter, argsParameter);
            var callback1 = lambda.Compile();

            Callback callback2 = (instance, args) =>
            {
                (instance as VoidCommand).Execute();

                return new ValueTask<IResult>(new SuccessResult() as IResult);
            };

            var result = await callback1(new VoidCommand(), new object[0]);
            var result2 = await callback2(new VoidCommand(), new object[0]);
        }
    }

    public class VoidCommand
    {
        public void Execute()
        {

        }
    }
}