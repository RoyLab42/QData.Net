using System;
using System.Linq;
using RoyLab.QData.Lite.Filter.Expressions;
using RoyLab.QData.Lite.Interfaces;

namespace RoyLab.QData.Lite.Filter
{
    internal static class FilterParser
    {
        public static IExpression Parse(string queryString)
        {
            return TryParse(queryString, out var expression, out _) ? expression : null;
        }

        public static bool TryParse(string queryString, out IExpression expression, out ReadOnlySpan<char> remaining)
        {
            expression = null;
            remaining = Parse(queryString, ref expression, out var isError);
            if (remaining.Length > 0)
            {
                var i = 0;
                while (i < remaining.Length && remaining[i] == ' ')
                {
                    i++;
                }

                if (i != remaining.Length)
                {
                    isError = true;
                }

                remaining = remaining[i..];
            }

            return !isError;
        }

        private static ReadOnlySpan<char> Parse(ReadOnlySpan<char> queryString, ref IExpression expression,
            out bool isError)
        {
            var remaining = ReadOnlySpan<char>.Empty;
            isError = false;

            var i = 0;
            while (i < queryString.Length && queryString[i] == ' ')
            {
                i++;
            }

            if (i == queryString.Length)
            {
                return remaining;
            }

            switch (queryString[i])
            {
                case '(':
                    remaining = Parse(queryString[(i + 1)..], ref expression, out isError);
                    if (isError)
                    {
                        return remaining;
                    }

                    i = 0;
                    while (i < remaining.Length && remaining[i] == ' ')
                    {
                        i++;
                    }

                    if (i == remaining.Length || remaining[i] != ')')
                    {
                        isError = true;
                        return remaining;
                    }

                    return remaining[(i + 1)..];
                case '|':
                case '&':
                    IExpression left = null;
                    IExpression right = null;
                    remaining = Parse(queryString[(i + 1)..], ref left, out isError);
                    if (isError)
                    {
                        return remaining;
                    }

                    remaining = Parse(remaining, ref right, out isError);
                    if (isError)
                    {
                        return remaining;
                    }

                    expression = queryString[i] switch
                    {
                        '|' => new OrExpression(left, right) as IExpression,
                        '&' => new AndExpression(left, right) as IExpression,
                        _ => null
                    };
                    return remaining;
                case '!':
                    IExpression single = null;
                    remaining = Parse(queryString[(i + 1)..], ref single, out isError);
                    if (isError)
                    {
                        return remaining;
                    }

                    expression = new NotExpression(single);
                    return remaining;
                default:
                    var j = i;
                    while (j < queryString.Length && queryString[j] != ' ' &&
                           queryString[j] != '=' && queryString[j] != '>' && queryString[j] != '<')
                    {
                        j++;
                    }

                    if (j == queryString.Length)
                    {
                        isError = true;
                        return ReadOnlySpan<char>.Empty;
                    }

                    var variable = queryString[i..j].ToString();
                    while (j < queryString.Length && queryString[j] == ' ')
                    {
                        j++;
                    }

                    if (j + 1 >= queryString.Length)
                    {
                        isError = true;
                        return ReadOnlySpan<char>.Empty;
                    }

                    var inOperation = false;
                    var operation = Operation.Na;
                    switch (queryString[j++])
                    {
                        case '=':
                            operation = Operation.Eq;
                            break;
                        case '>':
                            if (queryString[j] == '=')
                            {
                                operation = Operation.Ge;
                                j++;
                            }
                            else
                            {
                                operation = Operation.Gt;
                            }

                            break;
                        case '<':
                            if (queryString[j] == '=')
                            {
                                operation = Operation.Le;
                                j++;
                            }
                            else
                            {
                                operation = Operation.Lt;
                            }

                            break;
                        case 'i':
                            if (queryString[j] == 'n')
                            {
                                inOperation = true;
                                j++;
                            }

                            break;
                        default:
                            isError = true;
                            return queryString[j..];
                    }

                    if (inOperation)
                    {
                        while (j < queryString.Length && queryString[j] == ' ')
                        {
                            j++;
                        }

                        if (j == queryString.Length)
                        {
                            isError = true;
                            return queryString;
                        }

                        if (queryString[j] != '[')
                        {
                            isError = true;
                            return queryString[j..];
                        }

                        j++;
                        i = j;
                        while (j < queryString.Length && queryString[j] != ']' && queryString[j] != ')')
                        {
                            j++;
                        }

                        if (j == queryString.Length)
                        {
                            isError = true;
                            return ReadOnlySpan<char>.Empty;
                        }

                        if (queryString[j] != ']')
                        {
                            isError = true;
                            return queryString[j..];
                        }

                        var value = queryString[i..j].ToString().Split(',')
                            .Select(s => s?.Trim())
                            .ToArray();
                        expression = new InExpression(variable, value);
                        return queryString[(j + 1)..];
                    }
                    else
                    {
                        while (j < queryString.Length && queryString[j] == ' ')
                        {
                            j++;
                        }

                        if (j == queryString.Length)
                        {
                            isError = true;
                            return queryString;
                        }

                        i = j;
                        while (j < queryString.Length && queryString[j] != ' ' && queryString[j] != ')')
                        {
                            j++;
                        }

                        var value = queryString[i..j].ToString();
                        expression = new CompareExpression(variable, value, operation);
                        return queryString[j..];
                    }
            }
        }
    }
}