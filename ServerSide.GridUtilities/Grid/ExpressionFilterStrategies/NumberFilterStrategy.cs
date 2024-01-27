using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid.Constants;
using System.Linq.Expressions;

namespace ServerSide.GridUtilities.Grid.ExpressionFilterStrategies;

public class NumberFilterStrategy : IFilterStrategy
{
    private readonly FilterMethod[] AVAILABLE_FILTER_METHODS =
    [
        FilterMethod.Equals,
        FilterMethod.NotEqual,
        FilterMethod.GreaterThan,
        FilterMethod.LessThan,
        FilterMethod.GreaterThanOrEqual,
        FilterMethod.LessThanOrEqual,
        FilterMethod.InRange,
        FilterMethod.Blank,
        FilterMethod.NotBlank
    ];

    public Expression? GetExpression<T>(MemberExpression selector, FilterMethod filterMethod, string?[] filterValues)
    {
        if (AVAILABLE_FILTER_METHODS.All(t => t != filterMethod))
        {
            throw new NotSupportedException($"Filter method: {filterMethod.GetDescription()} not supported on type Number");
        }

        var isNullable = Nullable.GetUnderlyingType(selector.Type) != null;
        var member = isNullable
            ? Expression.PropertyOrField(selector, "Value")
            : selector;

        Expression? expression = null;

        decimal? numberFrom = default;
        decimal? numberTo = default;

        if (filterValues.Length > 0)
        {
            numberFrom = decimal.TryParse(filterValues[0], out decimal number) ? number : null;
        }

        if (filterValues.Length > 1)
        {
            numberTo = decimal.TryParse(filterValues[1], out decimal number) ? number : null;
        }

        if (numberFrom.HasValue && (filterMethod == FilterMethod.Equals || filterMethod == FilterMethod.NotEqual))
        {
            var filterExpr = Expression.Convert(
                Expression.Constant(numberFrom.Value, numberFrom.Value.GetType()),
                member.Type
            );
            expression = filterMethod == FilterMethod.Equals
                ? Expression.Equal(member, filterExpr)
                : Expression.NotEqual(member, filterExpr);
        }

        if (numberFrom.HasValue && (filterMethod == FilterMethod.LessThan || filterMethod == FilterMethod.GreaterThan))
        {

            var filterExpr = Expression.Convert(
                Expression.Constant(numberFrom.Value, numberFrom.Value.GetType()),
                member.Type
            );
            expression = filterMethod == FilterMethod.LessThan
                ? Expression.LessThan(member, filterExpr)
                : Expression.GreaterThan(member, filterExpr);
        }

        if (numberFrom.HasValue && (filterMethod == FilterMethod.LessThanOrEqual || filterMethod == FilterMethod.GreaterThanOrEqual))
        {
            var filterExpr = Expression.Convert(
                Expression.Constant(numberFrom.Value, numberFrom.Value.GetType()),
                member.Type
            );
            expression = filterMethod == FilterMethod.LessThanOrEqual
                ? Expression.LessThanOrEqual(member, filterExpr)
                : Expression.GreaterThanOrEqual(member, filterExpr);
        }

        if (numberFrom.HasValue && numberTo.HasValue && filterMethod == FilterMethod.InRange)
        {
            var filterExprFrom = Expression.Convert(
                Expression.Constant(numberFrom.Value, numberFrom.Value.GetType()),
                member.Type
            );
            var filterExpTo = Expression.Convert(
               Expression.Constant(numberTo.Value, numberTo.Value.GetType()),
               member.Type
            );

            expression = Expression.AndAlso(
                                    Expression.GreaterThanOrEqual(member, filterExprFrom),
                                    Expression.LessThanOrEqual(member, filterExpTo)
                               );
        }

        if (filterMethod == FilterMethod.Blank || filterMethod == FilterMethod.NotBlank)
        {
            if (isNullable)
            {
                var hasValueMember = Expression.PropertyOrField(selector, "HasValue");
                var expressionFalse = Expression.Constant(false, typeof(bool));

                expression = filterMethod == FilterMethod.Blank
                    ? Expression.Equal(hasValueMember, expressionFalse)
                    : Expression.NotEqual(hasValueMember, expressionFalse);
            }
            else
            {
                var expr = Expression.Convert(
                    Expression.Constant(long.MinValue, typeof(long)),
                    member.Type
                );
                expression = Expression.GreaterThan(member, expr);
            }
        }

        return expression;
    }
}
