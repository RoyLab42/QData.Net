using System.Collections.Generic;
using RoyLab.QData.Interfaces;

namespace RoyLab.QData.Filter.Expressions
{
    internal class InExpression : IExpression
    {
        private readonly string[] valueList;

        public InExpression(string variable, string[] valueList)
        {
            Variable = variable;
            this.valueList = valueList;
        }

        public override string ToString()
        {
            return $"({Variable}_in_)";
        }

        public string Variable { get; }

        public IEnumerable<string> ValueList => valueList;

        public T Accept<T>(IExpressionVisitor<T> expressionVisitor)
        {
            return expressionVisitor.VisitAndConvert(this);
        }
    }
}