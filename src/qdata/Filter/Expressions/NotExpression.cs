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

        public T Accept<T>(IExpressionVisitor<T> expressionVisitor)
        {
            return expressionVisitor.VisitAndConvert(this);
        }
    }
}