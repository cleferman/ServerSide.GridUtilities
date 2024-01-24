using ServerSide.GridUtilities.Grid.Constants;
using ServerSide.GridUtilities.Grid.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ServerSide.GridUtilities.Grid;

public sealed class GridRequest : GridRequest<Pagination>
{
    public override Pagination Pagination { get; set; } = null!;
}

public interface IPagination { }
public abstract class GridRequest<T> where T : IPagination
{
    public abstract T Pagination { get; set; }

    public IList<string> Columns { get; set; } = Array.Empty<string>();

    public IList<SortModel> Sorting { get; set; } = Array.Empty<SortModel>();

    public IList<FilterModel> Filtering { get; set; } = Array.Empty<FilterModel>();

    public GroupingModel Grouping { get; set; } = null!;
}

public class Pagination : IPagination
{   
    public int StartRow { get; set; }
    public int EndRow { get; set; }
}

/// <summary>
/// Represents the sort type for sorting columns.
/// 0 for Ascending, 1 for Descending.
/// </summary>
[JsonConverter(typeof(SortEnumJsonConverter))]
public enum SortType
{
    /// <summary>
    /// Sort in ascending order.
    /// </summary>
    [Description("asc")]
    Ascending,

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    [Description("desc")]
    Descending
}


public class SortModel
{
    public string ColName { get; set; } = string.Empty;

    public SortType Sort { get; set; }

    public string GetSort(bool reverseOrder = false)
    {
        if (reverseOrder)
        {
            return Sort == SortType.Descending ? "ASC" : "DESC";
        }
        else
        {
            return Sort == SortType.Descending ? "DESC" : "ASC";
        }
    }
}

public class FilterModel
{
    public Condition[] Conditions { get; set; } = [];

    public string FieldName { get; set; } = null!;

    public FilterType FilterType { get; set; }

    /// <summary>
    /// Null for a single condition filter.
    /// Specify the operator when you have two conditions and you want to apply conditional logic between them.
    /// </summary>
    public FilterOperator? Operator { get; set; }
}

public class Condition
{
    public FilterMethod FilterMethod { get; set; }

    public string?[] Values { get; set; } = [];
}

public class GroupingModel
{
    public RowGroupCol[] RowGroupCols { get; set; } = [];
    public string[] GroupKeys { get; set; } = [];
}

public class RowGroupCol
{
    public string ColName { get; set; } = null!;
    public string FilterType { get; set; } = null!;
}
