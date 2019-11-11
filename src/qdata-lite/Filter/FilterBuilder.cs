using System;
using System.Linq.Expressions;
using RoyLab.QData.Lite.Interfaces;

namespace RoyLab.QData.Lite.Filter
{
    internal static class FilterBuilder
    {
        /// <summary>
        /// return an LambdaExpression of Fun&lt;T, bool&gt; where T is sourceType
        /// </summary>
        /// <param name="iExpression"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static LambdaExpression Build(this IExpression iExpression, Type sourceType)
        {
            if (iExpression == null)
            {
                return null;
            }

            var parameterExpression = Expression.Parameter(sourceType);
            var expression = iExpression.ToLinqExpression(parameterExpression);
            return expression == null ? null : Expression.Lambda(expression, parameterExpression);
        }
    }
}