using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

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
    }
}