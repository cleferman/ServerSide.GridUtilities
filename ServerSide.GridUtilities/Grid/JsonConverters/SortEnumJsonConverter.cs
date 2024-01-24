using System.Text.Json;

namespace ServerSide.GridUtilities.Grid.JsonConverters
{
    public class SortEnumJsonConverter : EnumJsonConverter<SortType>
    {
        public override SortType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();
            
            if(value == "asc")
            {
                return SortType.Ascending;
            }
            
            if(value == "desc")
            {
                return SortType.Descending;
            }

           return  base.Read(ref reader, typeToConvert, options);
        }
    }
}
