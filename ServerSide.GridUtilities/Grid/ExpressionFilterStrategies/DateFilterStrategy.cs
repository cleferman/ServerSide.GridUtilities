using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid.Constants;
using System.Linq.Expressions;
using System.Reflection;

namespace ServerSide.GridUtilities.Grid.ExpressionFilterStrategies;
public class DateFilterStrategy : IFilterStrategy
{
    private readonly FilterMethod[] AVAILABLE_FILTER_METHODS =
    [
        FilterMethod.Equals,
        FilterMethod.NotEqual,
        FilterMethod.GreaterThan,
        FilterMethod.LessThan,
        FilterMethod.Blank,
        FilterMethod.NotBlank
    ];

    public Expression? GetExpression<T>(MemberExpression selector, FilterMethod filterMethod, string?[] filterValues)
    {
        if(AVAILABLE_FILTER_METHODS.All(t => t != filterMethod))
        {
            throw new NotSupportedException($"Filter method: {filterMethod.GetDescription()} not supported on type Date");
        }

        Expression? expression = null;

        DateTime? dateFrom = filterValues.Length > 0 && !string.IsNullOrEmpty(filterValues[0]) ? DateTime.Parse(filterValues[0]!) : default;
        DateTime? dateTo = filterValues.Length > 1 && !string.IsNullOrEmpty(filterValues[1]) ? DateTime.Parse(filterValues[1]!) : default(DateTime?);

        var isNullable = Nullable.GetUnderlyingType(selector.Type) != null;
        var dateMember = isNullable
            ? Expression.PropertyOrField(Expression.PropertyOrField(selector, "Value"), "Date")
            : Expression.PropertyOrField(selector, "Date");

        if (!dateFrom.HasValue) return Expression.Empty();

        if (dateTo.HasValue && filterMethod == FilterMethod.InRange)
        {
            var dateFromExpression = Expression.Constant(dateFrom.Value.Date, typeof(DateTime));
            var dateToExpression = Expression.Constant(dateTo.Value.Date, typeof(DateTime));

            expression = Expression.AndAlso(
                                        Expression.GreaterThan(dateMember, dateFromExpression),
                                        Expression.LessThan(dateMember, dateToExpression)
                               );
            if (isNullable)
            {
                var hasValueMember = Expression.PropertyOrField(selector, "HasValue");

                expression = Expression.AndAlso(hasValueMember, expression);
            }
        }

        if (filterMethod == FilterMethod.Equals || filterMethod == FilterMethod.NotEqual)
        {
            MethodInfo? method = typeof(DateTime).GetMethod("Equals", [typeof(DateTime)]);
            if (method != null)
            {
                var filterExpr = Expression.Constant(dateFrom.Value.Date, typeof(DateTime));

                expression = Expression.Call(dateMember, method, filterExpr);

                if (filterMethod == FilterMethod.NotEqual)
                {
                    expression = Expression.Not(expression);
                }
            }
        }

        if (filterMethod == FilterMethod.GreaterThan || filterMethod == FilterMethod.LessThan)
        {

            if (filterMethod == FilterMethod.GreaterThan)
            {
                var filterExpr = Expression.Constant(dateFrom.Value.AddDays(-1).Date, typeof(DateTime));
                expression = Expression.GreaterThan(dateMember, filterExpr);
            }
            else
            {
                var filterExpr = Expression.Constant(dateFrom.Value.AddDays(1).Date, typeof(DateTime));

                expression = Expression.LessThan(dateMember, filterExpr);
            }

            if (isNullable)
            {
                var hasValueMember = Expression.PropertyOrField(selector, "HasValue");

                expression = Expression.AndAlso(hasValueMember, expression);
            }
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
                var filterExpr = Expression.Constant(default(DateTime), typeof(DateTime));

                expression = Expression.GreaterThan(dateMember, filterExpr);
            }
        }

        return expression;
    }
}
