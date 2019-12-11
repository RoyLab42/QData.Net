using System.Linq;
using System.Linq.Expressions;
using RoyLab.QData.Filter.Expressions;
using RoyLab.QData.Interfaces;
using RoyLab.QData.Updater.Expressions;

namespace RoyLab.QData.Converters.ExpressionTrees
{
    internal class ExpressionTreesExpressionVisitor : IExpressionVisitor<Expression>
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

            var valueExpression = TypeUtility.Parse(Expression.Constant(compareExpression.Value), memberType);

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

            var valueArrayExpression = Expression.NewArrayInit(memberType,
                inExpression.ValueList.Select(v => TypeUtility.Parse(Expression.Constant(v), memberType)));

            var containsMethod = typeof(Enumerable).GetMethods()
                .FirstOrDefault(mi => mi.Name == "Contains" && mi.GetParameters().Length == 2)
                ?.MakeGenericMethod(memberType);
            return containsMethod == null
                ? null
                : Expression.Call(containsMethod, valueArrayExpression, memberExpression);
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

            return Expression.Assign(me, TypeUtility.TryParse(parameters[assignExpression.Index], mt));
        }
    }
}