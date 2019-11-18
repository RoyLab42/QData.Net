using System;
using System.Linq.Expressions;
using RoyLab.QData.Converters.ExpressionTrees;
using RoyLab.QData.Interfaces;

namespace RoyLab.QData.Filter
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
            var visitor = new ExpressionTreesExpressionVisitor(parameterExpression);
            var expression = iExpression.Accept(visitor);
            return expression == null ? null : Expression.Lambda(expression, parameterExpression);
        }
    }
}