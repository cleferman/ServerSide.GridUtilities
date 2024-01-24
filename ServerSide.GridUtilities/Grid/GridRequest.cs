using ServerSide.GridUtilities.Grid.Constants;

namespace ServerSide.GridUtilities.Grid;

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

public enum SortType
{
    Ascending,
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
    public Condition[] Conditions { get; set; } = Array.Empty<Condition>();

    public string FieldName { get; set; } = string.Empty;

    public FilterType FilterType { get; set; }

    public FilterOperator? Operator { get; set; }
}

public class Condition
{
    public FilterMethod FilterMethod { get; set; }

    public string?[] Values { get; set; } = Array.Empty<string>();
}

public class GroupingModel
{
    public RowGroupCol[] RowGroupCols { get; set; } = Array.Empty<RowGroupCol>();
    public string[] GroupKeys { get; set; } = Array.Empty<string>();
}

public class RowGroupCol
{
    public string ColName { get; set; } = null!;
    public string FilterType { get; set; } = null!;
}
