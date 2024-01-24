using ServerSide.GridUtilities.Grid.Constants;
using System.Linq.Expressions;

namespace ServerSide.GridUtilities.Grid.ExpressionFilterStrategies;

public interface IFilterStrategy
{
    static abstract FilterType METHOD { get; }

    Expression? GetExpression<T>(MemberExpression selector, FilterMethod filterMethod, string?[] filterValues);
}
