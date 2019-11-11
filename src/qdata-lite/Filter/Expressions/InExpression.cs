using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RoyLab.QData.Lite.Interfaces;

namespace RoyLab.QData.Lite.Filter.Expressions
{
    internal class InExpression : IExpression
    {
        private readonly string[] valueList;

        public InExpression(string variable, string[] valueList)
        {
            Variable = variable;
            this.valueList = valueList;
        }

        public override string ToString()
        {
            return $"({Variable}_in_)";
        }

        public string Variable { get; }

        public IEnumerable<string> ValueList => valueList;

        public Expression ToLinqExpression(params ParameterExpression[] parameters)
        {
            var source = parameters.First();
            var (memberType, memberExpression) = source.AccessPropertyOrMember(Variable);
            if (memberType == null || memberExpression == null)
            {
                return null;
            }

            var castMethod = typeof(Enumerable).GetMethod("Cast")?.MakeGenericMethod(memberType);
            if (castMethod == null)
            {
                return null;
            }

            var valueExpression =
                Expression.Call(castMethod,
                    Expression.Constant(valueList.Select(v => Convert.ChangeType(v,
                        memberType.IsEnum
                            ? Enum.GetUnderlyingType(memberType)
                            : memberType))));

            var containsMethod = typeof(Enumerable).GetMethods()
                .FirstOrDefault(mi => mi.Name == "Contains" && mi.GetParameters().Length == 2)
                ?.MakeGenericMethod(memberType);
            return containsMethod == null ? null : Expression.Call(containsMethod, valueExpression, memberExpression);
        }
    }
}