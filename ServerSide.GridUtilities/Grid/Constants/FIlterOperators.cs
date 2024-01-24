using ServerSide.GridUtilities.Grid.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ServerSide.GridUtilities.Grid.Constants;

public static class FilterOperators
{
    public const string And = "AND";
    public const string Or = "OR";
}

[JsonConverter(typeof(EnumJsonConverter<FilterOperator>))]
public enum FilterOperator
{
    [Description(FilterOperators.And)]
    And,
    [Description(FilterOperators.Or)]
    Or,
}