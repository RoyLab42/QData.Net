using RoyLab.QData.Interfaces;

namespace RoyLab.QData.Updater.Expressions
{
    internal class AssignExpression : IExpression
    {
        public AssignExpression(string variable, int index)
        {
            Variable = variable;
            Index = index;
        }

        public string Variable { get; }

        /// <summary>
        /// index of the AssignExpression in the list of assignments, start from 1
        /// </summary>
        public int Index { get; }

        public override string ToString()
        {
            return $"{Variable}=_";
        }

        public T Accept<T>(IExpressionVisitor<T> expressionVisitor)
        {
            return expressionVisitor.VisitAndConvert(this);
        }
    }
}