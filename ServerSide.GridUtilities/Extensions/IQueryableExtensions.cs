﻿using ServerSide.GridUtilities.Grid;
using System.Linq.Expressions;

namespace ServerSide.GridUtilities.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> Select<T>(this IQueryable<T> queryable, IList<string> columns)
    {
        if (columns.Count == 0) return queryable;

        var parameter = Expression.Parameter(typeof(T), "e");
        var bindings = columns
            .Select(name => Expression.PropertyOrField(parameter, name))
            .Select(member => Expression.Bind(member.Member, member));

        var body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
        var selector = Expression.Lambda<Func<T, T>>(body, parameter);
        return queryable.Select(selector);
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<SortModel> sortModels)
    {
        var expression = source.Expression;
        int count = 0;
        foreach (var item in sortModels)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, item.ColName);
            var method = item.Sort == SortType.Descending ?
                count == 0 ? "OrderByDescending" : "ThenByDescending" :
                count == 0 ? "OrderBy" : "ThenBy";
            expression = Expression.Call(typeof(Queryable), method,
                [source.ElementType, selector.Type],
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));
            count++;
        }
        return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
    }

    public static IQueryable<T> ThenOrderBy<T>(this IQueryable<T> source, IEnumerable<SortModel> sortModels)
    {
        var expression = source.Expression;
        int count = 0;
        foreach (var item in sortModels)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, item.ColName);
            var method = item.Sort == SortType.Descending ?
                "ThenByDescending" :
                "ThenBy";
            expression = Expression.Call(typeof(Queryable), method,
                [source.ElementType, selector.Type],
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));
            count++;
        }
        return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
    }

    public static IQueryable<T> FilterBy<T>(this IQueryable<T> source, IList<FilterModel> filterModels)
    {
        var expression = source.Expression;
        int count = 0;

        var filterHandler = new FilterHandler();

        foreach (var filterModel in filterModels)
        {
            filterHandler.SetStrategy(filterModel.FilterType);

            var isMultiCondition = filterModel.Operator is not null;

            var condition = filterModel.Conditions.First();

            LambdaExpression? lambdaExpression = !isMultiCondition
                ? filterHandler.GetExpression<T>(filterModel.FieldName, condition.FilterMethod, condition.Values)
                : filterHandler.GetExpression<T>(filterModel.FieldName, filterModel.Conditions, filterModel.Operator!.Value);

            if (lambdaExpression != null)
            {
                expression = Expression.Call(typeof(Queryable), "Where", new Type[] { source.ElementType },
                    expression,
                    Expression.Quote(lambdaExpression)
                );

                count++;
            }
        }

        return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
    }

    public static IQueryable<T> GroupBy<T>(this IQueryable<T> source, GroupingModel groupModel)
    {
        if (IsDoingGrouping(groupModel))
        {
            var groupByColumn = groupModel.RowGroupCols[groupModel.GroupKeys.Length].ColName;

            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, groupByColumn);

            Expression<Func<T, object>> keySelector = Expression.Lambda<Func<T, object>>
            (
                Expression.Convert(
                    Expression.PropertyOrField(parameter, groupByColumn),
                    typeof(object)
                ),
                parameter
            );

            var selectExpression = MapGroupToObject<IGrouping<object, T>, T>(new[] { selector.Member.Name }, groupByColumn);
            return source.GroupBy(keySelector).Select(selectExpression);
        }
        return source;
    }

    public static IQueryable<T> ApplyGridRequest<T>(this IQueryable<T> source, GridRequest gridRequest)
    {
        return source
           .Select(gridRequest.Columns)
           .FilterBy(gridRequest.Filtering)
           .OrderBy(gridRequest.Sorting)
           .Skip(gridRequest.Pagination.StartRow)
           .Take(gridRequest.Pagination.EndRow - gridRequest.Pagination.StartRow);
    }

    private static bool IsDoingGrouping(GroupingModel groupingModel)
    {
        return groupingModel.RowGroupCols.Length > groupingModel.GroupKeys.Length;
    }

    private static Expression<Func<TIn, TOut>> MapGroupToObject<TIn, TOut>(IEnumerable<string> propNames, string groupByColumn)
    {
        var outerParam = Expression.Parameter(typeof(TIn), "x");

        var type = typeof(TOut);
        var newType = Expression.New(type);
        var bindings = type
            .GetProperties()
            .Where(p => propNames.Contains(p.Name, StringComparer.InvariantCultureIgnoreCase))
            .Select(propertyInfo =>
            {
                if (string.Equals(propertyInfo.Name, groupByColumn, StringComparison.InvariantCultureIgnoreCase))
                {
                    var keyProp = Expression.PropertyOrField(outerParam, "Key");
                    return Expression.Bind(propertyInfo, Expression.Convert(keyProp, propertyInfo.PropertyType));
                }
                else
                {
                    var innerParam = Expression.Parameter(type, "t");
                    //TODO: this needs a foreach propName. And sum/average whatever else handled with a switch
                    var property = Expression.PropertyOrField(innerParam, "A029_SIIValue");

                    return Expression.Bind(
                                    propertyInfo,
                                    Expression.Convert(
                                        //e.g: x.Sum(t => t.A029_SIIValue)
                                        Expression.Call(
                                            typeof(Enumerable),
                                            "Sum",
                                            new[] { type },
                                            outerParam, Expression.Lambda(property, innerParam)
                                        ),
                                        propertyInfo.PropertyType
                                    )
                                );
                }
            });

        var member = Expression.MemberInit(newType, bindings);
        return Expression.Lambda<Func<TIn, TOut>>(member, outerParam);
    }
}