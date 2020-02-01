using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace RoyLab.QData.Converters.ExpressionTrees
{
    internal static class TypeUtility
    {
        /// <summary>
        /// TryParse functions built at runtime
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Delegate> tryParseFunctions =
            new ConcurrentDictionary<Type, Delegate>();

        public static Delegate GetTryParseFunction(Type outputType)
        {
            if (!tryParseFunctions.ContainsKey(outputType) &&
                !tryParseFunctions.TryAdd(outputType, BuildTryParse(outputType).Compile()))
            {
                return null;
            }

            return tryParseFunctions[outputType];
        }

        /// <summary>
        /// <para>
        /// Return an expression which try to parse the input string into outputType,
        /// if parse success, return the parsed value
        /// otherwise, return the default value of outputType
        /// </para>
        /// 
        /// <remarks>Notice: Don't let Entity Framework to consume this expression, use the compiled delegate to pre-execute
        /// client side code instead.
        /// See <see cref="GetTryParseFunction"/>
        /// </remarks>
        /// </summary>
        /// <param name="inputExpression"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
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
            var returnValueExpression = Expression.Variable(outputType);

            return Expression.Block(new[] {valueExpression, returnValueExpression},
                Expression.IfThenElse(Expression.Call(parseMethod, inputExpression, valueExpression),
                    Expression.Assign(returnValueExpression, Expression.Convert(valueExpression, outputType)),
                    Expression.Assign(returnValueExpression, Expression.Default(outputType))),
                returnValueExpression);
        }

        private static LambdaExpression BuildTryParse(Type outputType)
        {
            var parameters = new[]
            {
                Expression.Parameter(typeof(string)) // string parameter to parse
            };

            var body = TryParse(parameters[0], outputType);
            return Expression.Lambda(body, parameters);
        }
    }
}