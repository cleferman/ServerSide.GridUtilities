using ServerSide.GridUtilities.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServerSide.GridUtilities.Grid.JsonConverters
{
    public class EnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (Enum.TryParse(value, true, out T filterType))
            {
                return filterType;
            }
            else
            {
                // Handle the case when the value cannot be parsed to the enum
                // You can throw an exception or return a default value, depending on your requirements
                throw new JsonException($"Invalid {filterType.GetType().Name} value: {value}");
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            T value,
            JsonSerializerOptions options)
        {
            var stringValue = value.GetDescription();
            writer.WriteStringValue(stringValue);
        }
    }
}
