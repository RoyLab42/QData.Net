using System;
using System.Linq;
using System.Linq.Expressions;
using RoyLab.QData.Filter.Expressions;
using RoyLab.QData.Interfaces;
using RoyLab.QData.Updater.Expressions;

namespace RoyLab.QData.Converters.ExpressionTrees
{
    public class ExpressionTreesExpressionVisitor : IExpressionVisitor<Expression>
    {
        private readonly ParameterExpression[] parameters;

        public ExpressionTreesExpressionVisitor(params ParameterExpression[] parameters)
        {
            this.parameters = parameters;
        }

        public Expression VisitAndConvert(IExpression expression)
        {
            return expression switch
            {
                AndExpression andExpression => VisitAndConvert(andExpression),
                CompareExpression compareExpression => VisitAndConvert(compareExpression),
                InExpression inExpression => VisitAndConvert(inExpression),
                NotExpression notExpression => VisitAndConvert(notExpression),
                OrExpression orExpression => VisitAndConvert(orExpression),
                AssignExpression assignExpression => VisitAndConvert(assignExpression),
                _ => null
            };
        }

        private Expression VisitAndConvert(AndExpression andExpression)
        {
            var leftExpression = andExpression.Left.Accept(this);
            var rightExpression = andExpression.Right.Accept(this);
            if (leftExpression == null || rightExpression == null)
            {
                return null;
            }

            return Expression.And(leftExpression, rightExpression);
        }

        private Expression VisitAndConvert(CompareExpression compareExpression)
        {
            var source = parameters.First();
            var (memberType, memberExpression) = source.AccessPropertyOrMember(compareExpression.Variable);
            if (memberType == null || memberExpression == null)
            {
                return null;
            }

            Expression valueExpression = Expression.Constant(compareExpression.Value);
            if (memberType.IsEnum)
            {
                var underlyingExpression = TypeUtility.ParseString(valueExpression, Enum.GetUnderlyingType(memberType));
                valueExpression = Expression.Convert(underlyingExpression, memberType);
            }
            else if (memberType != typeof(string))
            {
                valueExpression = TypeUtility.ParseString(valueExpression, memberType);
            }

            return compareExpression.Operation switch
            {
                Operation.Eq => Expression.Equal(memberExpression, valueExpression),
                Operation.Ge => Expression.GreaterThanOrEqual(memberExpression, valueExpression),
                Operation.Gt => Expression.GreaterThan(memberExpression, valueExpression),
                Operation.Le => Expression.LessThanOrEqual(memberExpression, valueExpression),
                Operation.Lt => Expression.LessThan(memberExpression, valueExpression),
                _ => null
            };
        }

        private Expression VisitAndConvert(InExpression inExpression)
        {
            var source = parameters.First();
            var (memberType, memberExpression) = source.AccessPropertyOrMember(inExpression.Variable);
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
                    Expression.Constant(inExpression.ValueList.Select(v => Convert.ChangeType(v,
                        memberType.IsEnum
                            ? Enum.GetUnderlyingType(memberType)
                            : memberType))));

            var containsMethod = typeof(Enumerable).GetMethods()
                .FirstOrDefault(mi => mi.Name == "Contains" && mi.GetParameters().Length == 2)
                ?.MakeGenericMethod(memberType);
            return containsMethod == null ? null : Expression.Call(containsMethod, valueExpression, memberExpression);
        }

        private Expression VisitAndConvert(NotExpression notExpression)
        {
            var expression = notExpression.Single.Accept(this);
            return expression == null ? null : Expression.Not(expression);
        }

        private Expression VisitAndConvert(OrExpression orExpression)
        {
            var leftExpression = orExpression.Left.Accept(this);
            var rightExpression = orExpression.Right.Accept(this);
            if (leftExpression == null || rightExpression == null)
            {
                return null;
            }

            return Expression.Or(leftExpression, rightExpression);
        }

        private Expression VisitAndConvert(AssignExpression assignExpression)
        {
            var source = parameters.First();
            var (mt, me) = source.AccessPropertyOrMember(assignExpression.Variable);
            if (mt == null || me == null)
            {
                return null;
            }

            Expression valueExpression = parameters[assignExpression.Index];
            if (mt.IsEnum)
            {
                var underlyingExpression = TypeUtility.TryParse(valueExpression, Enum.GetUnderlyingType(mt));
                valueExpression = Expression.Convert(underlyingExpression, mt);
            }
            else if (mt != typeof(string))
            {
                valueExpression = TypeUtility.TryParse(valueExpression, mt);
            }

            return Expression.Assign(me, valueExpression);
        }
    }
}