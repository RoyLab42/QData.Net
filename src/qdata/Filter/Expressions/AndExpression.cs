using RoyLab.QData.Interfaces;

namespace RoyLab.QData.Filter.Expressions
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

        public T Accept<T>(IExpressionVisitor<T> expressionVisitor)
        {
            return expressionVisitor.VisitAndConvert(this);
        }
    }
}