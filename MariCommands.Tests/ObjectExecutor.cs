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

            var valueTaskCtor = ExpressionHelper.GetValueTaskCtor();
            var objectResultCtor = typeof(SuccessObjectResult).GetFirstCtor();

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var paramInfos = method.GetParameters();
            var parameters = ExpressionHelper.GetParameterExpressions(paramInfos, argsParameter);

            var instanceCast = Expression.Convert(instanceParameter, type);

            var resultVarExp = Expression.Variable(typeof(object), "result");
            var methodCall = Expression.Call(instanceCast, method, parameters);
            var methodAssign = Expression.Assign(resultVarExp, methodCall);
            var objectResultExpression = Expression.New(objectResultCtor, methodAssign);
            var convertExpression = Expression.Convert(objectResultExpression, typeof(IResult));

            var resultExpression = Expression.New(valueTaskCtor, convertExpression);

            var body = Expression.Block(
                new ParameterExpression[] { resultVarExp },
                resultExpression
            );

            var lambda = Expression.Lambda<Callback>(body, instanceParameter, argsParameter);
            var callback2 = lambda.Compile();

            Callback callback1 = (instance, args) =>
            {
                var result = (instance as ObjectCommand).Execute();

                return new ValueTask<IResult>(new SuccessObjectResult(result) as IResult);
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