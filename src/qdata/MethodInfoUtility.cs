using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RoyLab.QData
{
    internal static class MethodInfoUtility
    {
        /// <summary>
        /// Enumberable.Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        /// </summary>
        public static readonly MethodInfo EnumerableContainsGeneric = typeof(Enumerable).GetMethod("Contains",
            new[]
            {
                typeof(IEnumerable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
                Type.MakeGenericMethodParameter(0)
            });

        /// <summary>
        /// Enum.Parse<TEnum> (string)
        /// </summary>
        public static readonly MethodInfo EnumParseGeneric = typeof(Enum).GetMethod("Parse",
            new[]
            {
                typeof(string)
            });

        /// <summary>
        /// Enum.TryParse<TEnum> (string, out TEnum)
        /// </summary>
        public static readonly MethodInfo EnumTryParseGeneric = typeof(Enum).GetMethod("TryParse",
            new[]
            {
                typeof(string),
                Type.MakeGenericMethodParameter(0).MakeByRefType()
            });
    }
}