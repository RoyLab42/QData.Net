using System.Linq;
using System.Linq.Expressions;
using RoyLab.QData.Interfaces;

namespace RoyLab.QData.Filter.Expressions
{
    internal class NotExpression : IExpression
    {
        public NotExpression(IExpression single)
        {
            Single = single;
        }

        public IExpression Single { get; }

        public override string ToString()
        {
            return $"(!{Single})";
        }

        public Expression ToLinqExpression(params ParameterExpression[] parameters)
        {
            var expression = Single.ToLinqExpression(parameters.First());
            return expression == null ? null : Expression.Not(expression);
        }
    }
}