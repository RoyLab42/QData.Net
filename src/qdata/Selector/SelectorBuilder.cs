using System;
using System.Linq;
using System.Linq.Expressions;

namespace RoyLab.QData.Selector
{
    internal static class SelectorBuilder
    {
        /// <summary>
        /// Project sourceType into outputType with selector provided
        /// e.g.
        ///   - sourceType : class Student { string Name{ get; set; } int Age { get; set; } int Grade { get; set; } }
        ///   - selector   : Name,Age
        ///   - outputType will be an anonymous type:
        ///     - class anonymousType { int Age { get; set; } string Name{ get; set; } }
        ///     - Notice, properties were sorted by property name
        /// </summary>
        /// <param name="sourceType">the source type</param>
        /// <param name="selector">the selector string, separated by comma ","</param>
        /// <param name="outputType">the output anonymous type</param>
        /// <returns>Expression&lt;Func&lt;T, object&gt;&gt;</returns>
        public static LambdaExpression Build(Type sourceType, string selector, out Type outputType)
        {
            if (sourceType == null || selector == null)
            {
                outputType = null;
                return null;
            }

            var sourceProperties = selector
                .Split(",")
                .Where(fieldName => !string.IsNullOrWhiteSpace(fieldName))
                .Distinct()
                .ToDictionary(name => name, sourceType.GetProperty);
            outputType = RuntimeTypeBuilder.BuildDynamicType(sourceProperties.Values);
            if (outputType == null)
            {
                return null;
            }

            var sourceItem = Expression.Parameter(sourceType);
            var bindings = outputType.GetProperties()
                .Select(p => Expression.Bind(p, Expression.Property(sourceItem, sourceProperties[p.Name])))
                .OfType<MemberBinding>();

            var constructor = outputType.GetConstructor(Type.EmptyTypes);
            return constructor == null
                ? null
                : Expression.Lambda(Expression.MemberInit(Expression.New(constructor), bindings), sourceItem);
        }
    }
}