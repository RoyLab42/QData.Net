using System;
using System.Linq;
using System.Linq.Expressions;
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

        public Expression ToLinqExpression(params ParameterExpression[] parameters)
        {
            var source = parameters.First();
            var (mt, me) = source.AccessPropertyOrMember(Variable);
            if (mt == null || me == null)
            {
                return null;
            }

            Expression valueExpression = parameters[Index];
            if (mt.IsEnum)
            {
                var underlyingExpression = TypeUtility.TryParse(valueExpression, Enum.GetUnderlyingType(mt));
                valueExpression = Expression.Convert(underlyingExpression, mt);
            }
            else if (mt != typeof(string))
            {
                valueExpression = TypeUtility.TryParse(valueExpression, mt);
            }

            return Expression.Assign(me, valueExpression);
        }
    }
}