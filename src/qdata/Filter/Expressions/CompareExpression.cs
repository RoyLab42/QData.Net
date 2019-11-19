using RoyLab.QData.Interfaces;

namespace RoyLab.QData.Filter.Expressions
{
    internal enum Operation
    {
        Na, // unknown
        Gt, // >
        Lt, // <
        Eq, // =
        Ge, // >=
        Le // <=
    }

    internal class CompareExpression : IExpression
    {
        private readonly string variable;
        private readonly string value;
        private readonly Operation operation;

        public CompareExpression(string variable, string value, Operation operation)
        {
            this.variable = variable;
            this.value = value;
            this.operation = operation;
        }

        public string Variable => variable;
        public string Value => value;
        public Operation Operation => operation;

        public override string ToString()
        {
            return $"({variable}_{operation}_)";
        }

        public T Accept<T>(IExpressionVisitor<T> expressionVisitor)
        {
            return expressionVisitor.VisitAndConvert(this);
        }
    }
}