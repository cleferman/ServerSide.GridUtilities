using ServerSide.GridUtilities.Grid.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ServerSide.GridUtilities.Grid.Constants;

public class FilterTypes
{
    public const string Text = "text";
    public const string Date = "date";
    public const string Number = "number";
}


[JsonConverter(typeof(EnumJsonConverter<FilterType>))]
public enum FilterType
{
    [Description(FilterTypes.Text)]
    Text,
    [Description(FilterTypes.Date)]
    Date,
    [Description(FilterTypes.Number)]
    Number
}