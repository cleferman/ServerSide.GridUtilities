using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid;
using ServerSide.GridUtilities.Grid.Constants;
using System.Text.Json;

namespace ServerSide.GridUtilities.Tests
{
    public class FilterTypeTests
    {
        [Test]
        public void FilterTypeSerialization_AllMembers_Success()
        {
            foreach (FilterType filterType in Enum.GetValues(typeof(FilterType)))
            {
                var json = JsonSerializer.Serialize(filterType);
                Assert.That(json.Equals($"\"{filterType.GetDescription()}\""));
            }
        }

        [Test]
        public void FilterTypeDeSerialization_AllMembers_Success()
        {
            foreach (FilterType filterType in Enum.GetValues(typeof(FilterType)))
            {
                var filterModel = new FilterModel
                {
                    FieldName = "Name",
                    FilterType = filterType,
                    Conditions = []
                };

                var model = JsonSerializer.Deserialize<FilterModel>(JsonSerializer.Serialize(filterModel));

                Assert.IsNotNull(model);
                Assert.That(model.FilterType == filterType);
            }
        }

        [Test]
        public void FilterTypeDeSerialization_Set_InvalidFilterType()
        {
            var filterModel = "{\"Conditions\":[],\"FieldName\":\"Name\",\"FilterType\":\"set\",\"Operator\":null}";

            Assert.That(() => JsonSerializer.Deserialize<FilterModel>(filterModel), Throws.TypeOf<JsonException>().With.Message.EqualTo("Invalid FilterType value: set"));
        }
    }
}
