using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.Tests
{
    public static class ExpressionHelper
    {
        public static IEnumerable<Expression> GetParameterExpressions(ParameterInfo[] paramInfos, ParameterExpression argsParameter)
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

        public static MethodInfo GetTaskResultMethod()
        {
            return typeof(Task)
                    .GetMethods()
                    .Where(a => a.Name.Equals(nameof(Task.FromResult)))
                    .Where(a => a.IsStatic && a.IsPublic)
                    .Where(a => a.ContainsGenericParameters)
                    .FirstOrDefault().MakeGenericMethod(typeof(IResult));
        }

        public static async Task<IResult> ChangeResultAsync<TResult>(this Task<TResult> task)
            where TResult : IResult
        {
            return await task;
        }

        public static ConstructorInfo GetFirstCtor(this Type type)
            => type.GetConstructors().FirstOrDefault();
    }
}