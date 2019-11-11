using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RoyLab.QData.Lite.Updater.Expressions;

namespace RoyLab.QData.Lite.Updater
{
    internal static class UpdaterBuilder
    {
        /// <summary>
        /// build a function which assign properties/members of targetType
        /// </summary>
        /// <param name="assignExpressions"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static LambdaExpression Build(List<AssignExpression> assignExpressions, Type targetType)
        {
            if (targetType == null || assignExpressions == null || assignExpressions.Count == 0)
            {
                return null;
            }

            var parameters = new ParameterExpression[assignExpressions.Count + 1];
            parameters[0] = Expression.Parameter(targetType);
            for (var i = 1; i < parameters.Length; i++)
            {
                parameters[i] = Expression.Parameter(typeof(string));
            }

            var body = Expression.Block(assignExpressions
                .Select(ae => ae.ToLinqExpression(parameters))
                .Where(e => e != null));

            return Expression.Lambda(Expression.Block(body, Expression.Empty()), parameters);
        }
    }
}