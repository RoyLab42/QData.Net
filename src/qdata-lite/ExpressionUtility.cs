using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RoyLab.QData.Lite
{
    public static class ExpressionUtility
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
            Type memberType = null;
            MemberExpression memberExpression = null;

            var propertyInfo = parameterExpression.Type.GetProperty(variableName);
            MemberInfo memberInfo = propertyInfo;
            if (memberInfo != null)
            {
                memberType = propertyInfo.PropertyType;
                memberExpression = Expression.Property(parameterExpression, variableName);
            }
            else
            {
                var fieldInfo = parameterExpression.Type.GetField(variableName);
                memberInfo = fieldInfo;
                if (memberInfo != null)
                {
                    memberType = fieldInfo.FieldType;
                    memberExpression = Expression.Field(parameterExpression, variableName);
                }
            }

            return memberInfo == null ? (null, null) : (memberType, memberExpression);
        }
    }
}