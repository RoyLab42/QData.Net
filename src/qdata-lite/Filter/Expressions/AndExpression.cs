using System.Linq;
using System.Linq.Expressions;
using RoyLab.QData.Lite.Interfaces;

namespace RoyLab.QData.Lite.Filter.Expressions
{
    internal class AndExpression : IExpression
    {
        public AndExpression(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }

        public IExpression Left { get; }

        public IExpression Right { get; }

        public override string ToString()
        {
            return $"({Left}&&{Right})";
        }

        public Expression ToLinqExpression(params ParameterExpression[] parameters)
        {
            var source = parameters.First();
            var leftExpression = Left.ToLinqExpression(source);
            var rightExpression = Right.ToLinqExpression(source);
            if (leftExpression == null || rightExpression == null)
            {
                return null;
            }

            return Expression.And(leftExpression, rightExpression);
        }
    }
}