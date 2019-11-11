using System;
using System.Linq;
using System.Linq.Expressions;
using RoyLab.QData.Lite.Interfaces;

namespace RoyLab.QData.Lite.Filter.Expressions
{
    internal enum Operation
    {
        Na, // unknown
        Gt, // >
        Lt, // <
        Eq, // =
        Ge, // >=
        Le // <=
    }

    internal class CompareExpression : IExpression
    {
        private readonly string variable;
        private readonly string value;
        private readonly Operation operation;

        public CompareExpression(string variable, string value, Operation operation)
        {
            this.variable = variable;
            this.value = value;
            this.operation = operation;
        }

        public string Variable => variable;
        public string Value => value;
        public Operation Operation => operation;

        public override string ToString()
        {
            return $"({variable}_{operation}_)";
        }

        public Expression ToLinqExpression(params ParameterExpression[] parameters)
        {
            var source = parameters.First();
            var (memberType, memberExpression) = source.AccessPropertyOrMember(variable);
            if (memberType == null || memberExpression == null)
            {
                return null;
            }

            Expression valueExpression = Expression.Constant(value);
            if (memberType.IsEnum)
            {
                var underlyingExpression = TypeUtility.ParseString(valueExpression, Enum.GetUnderlyingType(memberType));
                valueExpression = Expression.Convert(underlyingExpression, memberType);
            }
            else if (memberType != typeof(string))
            {
                valueExpression = TypeUtility.ParseString(valueExpression, memberType);
            }

            return operation switch
            {
                Operation.Eq => Expression.Equal(memberExpression, valueExpression),
                Operation.Ge => Expression.GreaterThanOrEqual(memberExpression, valueExpression),
                Operation.Gt => Expression.GreaterThan(memberExpression, valueExpression),
                Operation.Le => Expression.LessThanOrEqual(memberExpression, valueExpression),
                Operation.Lt => Expression.LessThan(memberExpression, valueExpression),
                _ => null
            };
        }
    }
}