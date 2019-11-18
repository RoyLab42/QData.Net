using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using RoyLab.QData.Converters.ExpressionTrees;
using RoyLab.QData.Filter;
using RoyLab.QData.Selector;
using RoyLab.QData.Updater;

[assembly: InternalsVisibleTo("qdata.unittest")]

namespace RoyLab.QData
{
    public static class Utility
    {
        private static readonly ConcurrentDictionary<string, Delegate> updateFunctions =
            new ConcurrentDictionary<string, Delegate>();

        /// <summary>
        /// selector: Name,Age,Location
        /// orderBy:
        ///   - ascending  :  +Name,+Age
        ///   - descending :  -Name,-Age
        ///   - mixed (OrderBy, ThenBy) :  +Name,-Age
        /// query: &(Name=Roy)(Age=18)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static IQueryable QueryDynamic(this IQueryable source, string selector, string filter,
            string orderBy = null)
        {
            var outputType = source.ElementType;
            if (selector != null)
            {
                var selectorExpression = SelectorBuilder.Build(source.ElementType, selector, out outputType);
                if (selectorExpression != null)
                {
                    source = source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select",
                        new[] {source.ElementType, outputType},
                        source.Expression, selectorExpression));
                }
            }

            if (filter != null)
            {
                var queryExpression = FilterParser.Parse(filter).Build(outputType);
                if (queryExpression != null)
                {
                    source = source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where",
                        new[] {outputType},
                        source.Expression, queryExpression));
                }
            }

            if (orderBy != null)
            {
                var isFirstOrder = true;
                foreach (var orderByField in orderBy.Split(",").Where(of => of.Length > 1))
                {
                    var memberName = orderByField[1..];
                    var parameterExpression = Expression.Parameter(outputType);
                    var (t, e) = parameterExpression.AccessPropertyOrMember(memberName);
                    if (t == null || e == null)
                    {
                        continue;
                    }

                    var orderByExpression = orderByField[0] switch
                    {
                        '-' => Expression.Call(typeof(Queryable),
                            isFirstOrder ? "OrderByDescending" : "ThenByDescending",
                            new[] {outputType, t},
                            source.Expression, Expression.Lambda(e, parameterExpression)),
                        '+' => Expression.Call(typeof(Queryable),
                            isFirstOrder ? "OrderBy" : "ThenBy",
                            new[] {outputType, t},
                            source.Expression, Expression.Lambda(e, parameterExpression)),
                        _ => null
                    };

                    if (orderByExpression == null)
                    {
                        continue;
                    }

                    isFirstOrder = false;
                    source = source.Provider.CreateQuery(orderByExpression);
                }
            }

            return source;
        }

        public static bool TryUpdateDynamic(object target, string updateString)
        {
            if (target == null)
            {
                return false;
            }

            var isSuccess = UpdaterParser.TryParse(updateString, out var assignExpressions, out var valueArray);
            if (!isSuccess)
            {
                return false;
            }

            if (assignExpressions?.Count > 0)
            {
                var key = $"{target.GetType()}_{string.Join(";", assignExpressions)}";
                if (!updateFunctions.ContainsKey(key) &&
                    !updateFunctions.TryAdd(key, UpdaterBuilder.Build(assignExpressions, target.GetType()).Compile()))
                {
                    return false;
                }

                valueArray[0] = target;
                updateFunctions[key].DynamicInvoke(valueArray);
            }

            return true;
        }
    }
}