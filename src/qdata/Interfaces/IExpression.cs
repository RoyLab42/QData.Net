using System.Linq.Expressions;

namespace RoyLab.QData.Interfaces
{
    public interface IExpression
    {
        Expression ToLinqExpression(params ParameterExpression[] parameters);
    }
}