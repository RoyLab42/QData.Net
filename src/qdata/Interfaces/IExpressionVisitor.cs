namespace RoyLab.QData.Interfaces
{
    public interface IExpressionVisitor<out T>
    {
        T VisitAndConvert(IExpression expression);
    }
}