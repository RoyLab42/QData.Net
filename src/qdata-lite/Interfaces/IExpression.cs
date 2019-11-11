using System.Linq.Expressions;

namespace RoyLab.QData.Lite.Interfaces
{
    public interface IExpression
    {
        Expression ToLinqExpression(params ParameterExpression[] parameters);
    }
}