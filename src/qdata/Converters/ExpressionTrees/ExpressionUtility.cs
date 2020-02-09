using System;
using System.Linq.Expressions;

namespace RoyLab.QData.Converters.ExpressionTrees
{
    internal static class ExpressionUtility
    {
        /// <summary>
        /// access property or member of parameterExpression with the give property name or member name
        /// </summary>
        /// <param name="parameterExpression">the expression to access</param>
        /// <param name="variableName">
        ///   - Type : the property or member type
        ///   - MemberExpression : expression of the found property or member
        ///   Notice : (null, null) returned if parameterExpression does not contains
        ///                        the given property name and member name
        /// </param>
        /// <returns></returns>
        public static (Type, MemberExpression) AccessPropertyOrMember(this Expression parameterExpression,
            string variableName)
        {
            MemberExpression memberExpression = null;
            var memberType = parameterExpression.Type;

            foreach (var name in variableName.Split('.'))
            {
                var propertyInfo = memberType.GetProperty(name);
                if (propertyInfo != null)
                {
                    memberType = propertyInfo.PropertyType;
                    memberExpression = Expression.Property(memberExpression ?? parameterExpression, name);
                    continue;
                }

                var fieldInfo = memberType.GetField(name);
                if (fieldInfo == null)
                {
                    return (null, null);
                }

                memberType = fieldInfo.FieldType;
                memberExpression = Expression.Field(memberExpression ?? parameterExpression, name);
            }

            return (memberType, memberExpression);
        }
    }
}