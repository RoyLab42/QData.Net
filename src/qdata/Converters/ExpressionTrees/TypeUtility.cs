using System;
using System.Linq;
using System.Linq.Expressions;

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
        public static Expression Parse(Expression inputExpression, Type outputType)
        {
            if (outputType == typeof(string))
            {
                return inputExpression;
            }

            var realType = outputType;
            if (outputType.IsGenericType && outputType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                realType = outputType.GetGenericArguments().First();
            }

            var parseMethod = realType.IsEnum
                ? MethodInfoUtility.EnumParseGeneric.MakeGenericMethod(realType)
                : realType.GetMethod("Parse", new[] {typeof(string)});

            return parseMethod == null ? null : Expression.Call(parseMethod, inputExpression);
        }

        public static Expression TryParse(Expression inputExpression, Type outputType)
        {
            if (outputType == typeof(string))
            {
                return inputExpression;
            }

            var realType = outputType;
            if (outputType.IsGenericType && outputType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                realType = outputType.GetGenericArguments().First();
            }

            var parseMethod = realType.IsEnum
                ? MethodInfoUtility.EnumTryParseGeneric.MakeGenericMethod(realType)
                : realType.GetMethod("TryParse", new[] {typeof(string), realType.MakeByRefType()});

            if (parseMethod == null)
            {
                return null;
            }

            var valueExpression = Expression.Variable(realType);
            var tryParseResultExpression = Expression.Call(parseMethod, inputExpression, valueExpression);
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