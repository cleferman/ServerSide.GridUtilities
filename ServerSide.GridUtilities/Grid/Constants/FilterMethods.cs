using ServerSide.GridUtilities.Grid.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ServerSide.GridUtilities.Grid.Constants;

public static class FilterMethods
{
    public const string Contains = "contains";
    public const string NotContains = "notContains";
    public new const string Equals = "equals";
    public const string NotEqual = "notEqual";
    public const string StartsWith = "startsWith";
    public const string EndsWith = "endsWith";
    public const string Blank = "blank";
    public const string NotBlank = "notBlank";
    public const string LessThan = "lessThan";
    public const string GreaterThan = "greaterThan";
    public const string LessThanOrEqual = "lessThanOrEqual";
    public const string GreaterThanOrEqual = "greaterThanOrEqual";
    public const string InRange = "inRange";
    public const string In = "in";
    public const string NotIn = "notIn";
}

[JsonConverter(typeof(EnumJsonConverter<FilterMethod>))]
public enum FilterMethod
{
    [Description(FilterMethods.Contains)]
    Contains,
    [Description(FilterMethods.NotContains)]
    NotContains,
    [Description(FilterMethods.Equals)]
    Equals,
    [Description(FilterMethods.NotEqual)]
    NotEqual,
    [Description(FilterMethods.StartsWith)]
    StartsWith,
    [Description(FilterMethods.EndsWith)]
    EndsWith,
    [Description(FilterMethods.Blank)]
    Blank,
    [Description(FilterMethods.NotBlank)]
    NotBlank,
    [Description(FilterMethods.LessThan)]
    LessThan,
    [Description(FilterMethods.GreaterThan)]
    GreaterThan,
    [Description(FilterMethods.LessThanOrEqual)]
    LessThanOrEqual,
    [Description(FilterMethods.GreaterThanOrEqual)]
    GreaterThanOrEqual,
    [Description(FilterMethods.InRange)]
    InRange,
    [Description(FilterMethods.In)]
    In,
    [Description(FilterMethods.NotIn)]
    NotIn,
}
