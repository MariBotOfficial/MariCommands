using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MariCommands.Results;
using Xunit;

namespace MariCommands.Tests
{
    public class ObjectExecutor
    {
        [Fact]
        public async Task CanExecute()
        {
            var type = typeof(ObjectCommand);
            var method = type.GetMethod(nameof(ObjectCommand.Execute));

            var objectResultCtor = typeof(SuccessObjectResult).GetFirstCtor();

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var instanceCast = Expression.Convert(instanceParameter, type);

            var methodCall = Expression.Call(instanceCast, method, parameters);
            var objectResultExpression = Expression.New(objectResultCtor, methodCall);
            var convertExpression = Expression.Convert(objectResultExpression, typeof(IResult));

            var taskMethod = ExpressionHelper.GetTaskResultMethod();

            var taskResultExpression = Expression.Call(taskMethod, new Expression[] { convertExpression });

            var lambda = Expression.Lambda<Callback>(taskResultExpression, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                return Task.FromResult(new SuccessObjectResult((instance as ObjectCommand).Execute()) as IResult);
            };

            var result1 = await callback1(new ObjectCommand(), new object[0]);
            var result2 = await callback2(new ObjectCommand(), new object[0]);
        }
    }

    public class ObjectCommand
    {
        public object Execute()
        {
            return new object();
        }
    }

    public class SuccessObjectResult : IResult
    {
        public SuccessObjectResult(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public bool Success => true;

        public string Reason => null;

        public Exception Exception => null;
    }
}