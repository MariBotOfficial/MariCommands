using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.Utils
{
    internal static class ExecutorUtils
    {
        public static (Expression, ParameterExpression, ParameterExpression) GetBasicExpressions(
            IModuleBuilder moduleBuilder,
            ICommandBuilder commandBuilder
        )
        {
            const string instanceParameterName = "module";
            const string argsParameterName = "args";

            var instanceParameter = Expression.Parameter(typeof(object), instanceParameterName);
            var argsParameter = Expression.Parameter(typeof(object[]), argsParameterName);

            var paramInfos = commandBuilder.MethodInfo.GetParameters();
            var parameters = GetParameterExpressions(paramInfos, argsParameter);

            var instanceCast = Expression.Convert(instanceParameter, moduleBuilder.Type);
            var methodCall = Expression.Call(instanceCast, commandBuilder.MethodInfo, parameters);

            return (methodCall, instanceParameter, argsParameter);
        }

        internal static Expression AddAsTaskExpression(Expression body)
        {
            var asTaskMethod = body.Type.GetMethod(nameof(ValueTask.AsTask));

            return Expression.Call(body, asTaskMethod);
        }

        internal static Expression ChangeForObjectTaskResultExpression(Expression body, Type asyncResultType)
        {
            var convertMethod = typeof(ExecutorUtils)
                                    .GetMethod(nameof(ExecutorUtils.ChangeObjectResultAsync))
                                    .MakeGenericMethod(asyncResultType);

            return Expression.Call(convertMethod, new Expression[] { body });
        }

        public static Expression ChangeForTaskResultExpression(Expression body, Type asyncResultType)
        {
            var convertMethod = typeof(ExecutorUtils)
                                    .GetMethod(nameof(ExecutorUtils.ChangeResultAsync))
                                    .MakeGenericMethod(asyncResultType);

            return Expression.Call(convertMethod, new Expression[] { body });
        }

        public static async Task<IResult> ChangeResultAsync<TResult>(this Task<TResult> task)
            where TResult : IResult
        {
            return await task;
        }

        public static async Task<IResult> ChangeObjectResultAsync<TResult>(this Task<TResult> task)
        {
            var result = await task;

            return new SuccessObjectResult(result);
        }

        public static Expression AddContinueWithExpression(Expression body)
        {
            var continueWithMethod = ExecutorUtils.GetContinueWithMethod(body.Type);

            Expression<Func<Task, IResult>> continueWithBody = (task) => new SuccessResult();

            return Expression.Call(body, continueWithMethod, continueWithBody);
        }

        public static MethodInfo GetContinueWithMethod(System.Type type)
        {
            return type
                .GetMethods()
                .Where(a => a.IsPublic)
                .Where(a => a.Name.Equals(nameof(Task.ContinueWith)))
                .Where(a => a.ContainsGenericParameters)
                .Where(a => a.GetParameters().Count() == 1)
                .FirstOrDefault().MakeGenericMethod(typeof(IResult));
        }

        public static ConstructorInfo GetObjectResultCtor()
            => typeof(SuccessObjectResult).GetConstructors().FirstOrDefault();

        private static IEnumerable<Expression> GetParameterExpressions(ParameterInfo[] paramInfos, ParameterExpression argsParameter)
        {
            var parameters = new List<Expression>();

            for (var i = 0; i < paramInfos.Length; i++)
            {
                var param = paramInfos[i];
                var valueParam = Expression.ArrayIndex(argsParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueParam, param.ParameterType);

                parameters.Add(valueCast);
            }

            return parameters;
        }

        public static CommandCallback CreateCallback(Expression body, ParameterExpression instanceParameter, ParameterExpression argsParameter)
        {
            return Expression.Lambda<CommandCallback>(
                                body,
                                instanceParameter,
                                argsParameter)
                             .Compile();
        }

        public static CommandCallback CreateCallbackForTaskResult(Expression body, ParameterExpression instanceParameter, ParameterExpression argsParameter)
        {
            var taskMethod = GetTaskResultMethod();

            body = Expression.Call(taskMethod, new Expression[] { body });

            return CreateCallback(body, instanceParameter, argsParameter);
        }

        private static MethodInfo GetTaskResultMethod()
        {
            return typeof(Task)
                    .GetMethods()
                    .Where(a => a.Name.Equals(nameof(Task.FromResult)))
                    .Where(a => a.IsStatic && a.IsPublic)
                    .Where(a => a.ContainsGenericParameters)
                    .FirstOrDefault().MakeGenericMethod(typeof(IResult));
        }
    }
}