using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RoyLab.QData.Converters.ExpressionTrees
{
    internal static class TypeUtility
    {
        /// <summary>
        /// parse the inputExpression into outputType
        /// </summary>
        /// <param name="inputExpression"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public static Expression ParseString(Expression inputExpression, Type outputType)
        {
            var parseMethod = outputType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public,
                null, new[] {typeof(string)}, null);
            return Expression.Call(parseMethod, inputExpression);
        }

        public static Expression TryParse(Expression inputExpression, Type outputType)
        {
            var realType = outputType;
            if (outputType.IsGenericType && outputType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                realType = outputType.GetGenericArguments().First();
            }

            var valueExpression = Expression.Variable(realType);
            var tryParseMethod = realType.GetMethod("TryParse", new[] {typeof(string), realType.MakeByRefType()});
            var tryParseResultExpression = Expression.Call(tryParseMethod, inputExpression, valueExpression);

            var returnValueExpression = Expression.Variable(outputType);

            return Expression.Block(new[] {valueExpression, returnValueExpression},
                tryParseResultExpression,
                Expression.IfThenElse(tryParseResultExpression,
                    Expression.Assign(returnValueExpression, Expression.Convert(valueExpression, outputType)),
                    Expression.Assign(returnValueExpression, Expression.Default(outputType))),
                returnValueExpression);
        }
    }
}