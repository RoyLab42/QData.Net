using System;
using System.Collections.Generic;
using RoyLab.QData.Lite.Updater.Expressions;

namespace RoyLab.QData.Lite.Updater
{
    internal static class UpdaterParser
    {
        private const char Delimiter = ';';

        /// <summary>
        /// assign statements were separated by semi-colon ";"
        /// e.g. Name=Roy;Age=18
        /// Notice: semi-colon ";" escape "\" in value should be escaped by backslash "\"
        /// </summary>
        /// <param name="assignString"></param>
        /// <param name="assignExpressions"></param>
        /// <param name="valueArray"></param>
        /// <returns></returns>
        public static bool TryParse(ReadOnlySpan<char> assignString, out List<AssignExpression> assignExpressions,
            out object[] valueArray)
        {
            assignExpressions = null;
            valueArray = null;
            if (assignString == ReadOnlySpan<char>.Empty)
            {
                return true;
            }

            var values = new List<object> {null};

            var i = 0;
            var j = 0;
            var index = 1;
            while (i < assignString.Length)
            {
                while (i < assignString.Length && char.IsWhiteSpace(assignString[j]))
                {
                    i++;
                }

                if (j == assignString.Length)
                {
                    return false;
                }

                j = i;
                while (j < assignString.Length && !char.IsWhiteSpace(assignString[j]) &&
                       assignString[j] != '=')
                {
                    j++;
                }

                if (j == assignString.Length)
                {
                    return false;
                }

                var variable = assignString[i..j];
                while (j < assignString.Length && char.IsWhiteSpace(assignString[j]))
                {
                    j++;
                }

                if (assignString[j++] != '=')
                {
                    return false;
                }

                while (j < assignString.Length && char.IsWhiteSpace(assignString[j]))
                {
                    j++;
                }

                i = j;
                while (j < assignString.Length && assignString[j] != Delimiter)
                {
                    if (assignString[j] == '\\')
                    {
                        j += 2;
                    }
                    else
                    {
                        j++;
                    }
                }

                var value = assignString[i..Math.Min(j, assignString.Length)];
                j++;
                i = j;

                if (assignExpressions == null)
                {
                    assignExpressions = new List<AssignExpression>();
                }

                assignExpressions.Add(new AssignExpression(variable.ToString(), index++));
                values.Add(value.ToString()
                    .Replace(@"\\", @"\")
                    .Replace(@"\;", ";"));
            }

            valueArray = values.ToArray();
            return true;
        }
    }
}