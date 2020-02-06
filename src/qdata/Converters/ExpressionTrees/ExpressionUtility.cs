using System;
using System.Linq.Expressions;
using System.Reflection;

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
            Type memberType = null;
            MemberExpression memberExpression = null;

            var variableNames = variableName.Split('.');
            var baseType = parameterExpression.Type;

            MemberInfo memberInfo = null;

            foreach (var v in variableNames)
            {
                var propertyInfo = baseType.GetProperty(v);

                memberInfo = propertyInfo;

                if (memberInfo != null)
                {
                    memberType = propertyInfo.PropertyType;

                    if (memberExpression == null)
                    {
                        memberExpression = Expression.Property(parameterExpression, v);
                    }
                    else
                    {
                        memberExpression = Expression.Property(memberExpression, v);
                    }

                    baseType = memberType;
                }
                else
                {
                    var fieldInfo = baseType.GetField(v);

                    memberInfo = fieldInfo;

                    if (memberInfo != null)
                    {
                        memberType = fieldInfo.FieldType;

                        if (memberExpression == null)
                        {
                            memberExpression = Expression.Field(parameterExpression, v);
                        }
                        else
                        {
                            memberExpression = Expression.Field(memberExpression, v);
                        }

                        baseType = memberType;
                    }
                }
            }

            return memberInfo == null ? (null, null) : (memberType, memberExpression);
        }
    }
}
