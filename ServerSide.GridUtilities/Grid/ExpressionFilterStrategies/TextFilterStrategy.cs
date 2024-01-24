using ServerSide.GridUtilities.Grid.Constants;
using System.Linq.Expressions;
using System.Reflection;

namespace ServerSide.GridUtilities.Grid.ExpressionFilterStrategies;
public class TextFilterStrategy : IFilterStrategy
{
    public static FilterType METHOD => FilterType.Text;

    public Expression? GetExpression<T>(MemberExpression selector, FilterMethod filterMethod, string?[] filterValues)
    {
        Expression? expression = null;

        string filterValue = filterValues.Length > 0
            ? filterValues[0] ?? string.Empty
            : string.Empty;

        if (filterMethod == FilterMethod.Contains || filterMethod == FilterMethod.NotContains)
        {
            MethodInfo? method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            if (method != null)
            {
                var filterExpr = Expression.Constant(filterValue.ToLower(), typeof(string));
                expression = filterMethod == FilterMethod.Contains
                    ? Expression.Call(Expression.Call(selector, "ToLower", null), method, filterExpr)
                    : Expression.Not(Expression.Call(Expression.Call(selector, "ToLower", null), method, filterExpr));
            }
        }

        if (filterMethod == FilterMethod.Equals || filterMethod == FilterMethod.NotEqual)
        {
            MethodInfo? method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
            if (method != null)
            {
                var filterExpr = Expression.Constant(filterValue, typeof(string));
                expression = filterMethod == FilterMethod.Equals
                    ? Expression.Call(selector, method, filterExpr)
                    : Expression.Not(Expression.Call(selector, method, filterExpr));
            }
        }

        if (filterMethod == FilterMethod.StartsWith || filterMethod == FilterMethod.EndsWith)
        {
            MethodInfo? method = typeof(string).GetMethod(
                filterMethod == FilterMethod.StartsWith
                    ? "StartsWith"
                    : "EndsWith",
                new[] { typeof(string) }
            );

            if (method != null)
            {
                var filterExpr = Expression.Constant(filterValue.ToLower(), typeof(string));
                expression = Expression.Call(Expression.Call(selector, "ToLower", null), method, filterExpr);
            }
        }

        if (filterMethod == FilterMethod.Blank || filterMethod == FilterMethod.NotBlank)
        {
            MethodInfo? method = typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) });
            if (method != null)
            {
                expression = filterMethod == FilterMethod.Blank
                    ? Expression.Call(method, selector)
                    : Expression.Not(Expression.Call(method, selector));
            }
        }

        if (filterMethod == FilterMethod.In || filterMethod == FilterMethod.NotIn)
        {
            Expression<Func<IEnumerable<string>, bool>> containsExpr = (IEnumerable<string> q) => q.Contains(null);
            var containsMethod = ((MethodCallExpression)containsExpr.Body).Method;

            if (containsMethod != null)
            {
                var filterValuesExpr = Expression.Constant(filterValues, typeof(IEnumerable<string>));

                //filterValues.Contains(x.c200)
                expression = Expression.Call(containsMethod, filterValuesExpr, selector);
             
                expression = filterMethod == FilterMethod.In
                    ? Expression.Call(containsMethod, filterValuesExpr, selector)
                    : Expression.Not(Expression.Call(containsMethod, filterValuesExpr, selector));
            }
        }

        return expression;
    }
}