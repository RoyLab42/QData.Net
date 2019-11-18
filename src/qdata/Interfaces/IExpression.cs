namespace RoyLab.QData.Interfaces
{
    public interface IExpression
    {
        T Accept<T>(IExpressionVisitor<T> expressionVisitor);
    }
}