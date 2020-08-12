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

        public static ConstructorInfo GetValueTaskCtor()
        {
            return typeof(ValueTask<IResult>)
                                    .GetConstructors()
                                    .FirstOrDefault(a => a.GetParameters().FirstOrDefault().ParameterType == typeof(IResult));
        }

        public static ConstructorInfo GetFirstCtor(this Type type)
            => type.GetConstructors().FirstOrDefault();
    }
}