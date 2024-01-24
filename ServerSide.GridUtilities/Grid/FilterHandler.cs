using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid.Constants;
using ServerSide.GridUtilities.Grid.ExpressionFilterStrategies;
using System.Linq.Expressions;

namespace ServerSide.GridUtilities.Grid;

public class FilterHandler
{
    private IFilterStrategy? filterStrategy;

    public void SetStrategy(FilterType filterType)
    {
        filterStrategy = filterType switch
        {
            FilterType.Text => new TextFilterStrategy(),
            FilterType.Number => new NumberFilterStrategy(),
            FilterType.Date => new DateFilterStrategy(),
            _ => throw new NotImplementedException($"Filter strategy '{filterType.GetDescription()}' not implemented."),
        };
    }

    public LambdaExpression? GetExpression<T>(string fieldName, FilterMethod filterMethod, string?[] filterValues)
    {
        if(filterStrategy == null) throw new ArgumentException("Filter strategy not set. Make sure to call SetStrategy first.");

        var parameter = Expression.Parameter(typeof(T), "x");
        var selector = Expression.PropertyOrField(parameter, fieldName);
        var expression = filterStrategy.GetExpression<T>(selector, filterMethod, filterValues);

        return expression != null ? Expression.Lambda(expression, parameter) : null;
    }

    public LambdaExpression? GetExpression<T>(string fieldName, Condition[] conditions, FilterOperator filterOperator)
    {
        if (filterStrategy == null) throw new ArgumentException("Filter strategy not set. Make sure to call SetStrategy first.");

        Expression? innerExpression = null;

        var parameter = Expression.Parameter(typeof(T), "x");
        var selector = Expression.PropertyOrField(parameter, fieldName);

        for (var i = 0; i < conditions.Length; i++)
        {
            var condition = conditions[i];

            var expression = filterStrategy.GetExpression<T>(selector, condition.FilterMethod, condition.Values);

            if (i == 0)
            {
                innerExpression = expression;
            }

            if (i == 1 && innerExpression != null && expression != null)
            {
                innerExpression = filterOperator == FilterOperator.And
                    ? Expression.AndAlso(innerExpression, expression)
                    : Expression.OrElse(innerExpression, expression);
            }
        }
        return innerExpression != null ? Expression.Lambda(innerExpression, parameter) : null;
    }
}
