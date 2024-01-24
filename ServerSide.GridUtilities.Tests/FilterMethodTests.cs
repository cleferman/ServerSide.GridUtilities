using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid;
using ServerSide.GridUtilities.Grid.Constants;
using System.Text.Json;

namespace ServerSide.GridUtilities.Tests
{
    public class FilterMethodSerializationTests
    {
        [Test]
        public void FilterMethodSerialization_AllMembers_Success()
        {
            foreach (FilterMethod filterMethod in Enum.GetValues(typeof(FilterMethod)))
            {
                var json = JsonSerializer.Serialize(filterMethod);

                Assert.That(json.Equals($"\"{filterMethod.GetDescription()}\""));
            }
        }

        [Test]
        public void FilterMethodDeSerialization_AllMembers_Success()
        {
            foreach (FilterMethod filterMethod in Enum.GetValues(typeof(FilterMethod)))
            {
                var filterModel = new FilterModel
                {
                    FieldName = "Name",
                    FilterType = FilterType.Text,
                    Conditions = [new Condition { FilterMethod = filterMethod, Values = [] }]
                };

                var model = JsonSerializer.Deserialize<FilterModel>(JsonSerializer.Serialize(filterModel));

                Assert.IsNotNull(model);
                Assert.That(model.Conditions[0].FilterMethod == filterMethod);
            }
        }

        [Test]
        public void FilterMethodDeSerialization_InvalidFilterMethod()
        {
            var filterModel = "{\"Conditions\":[{\"FilterMethod\": \"max\", \"Values\":[]}],\"FieldName\":\"Name\",\"FilterType\":\"text\",\"Operator\":null}";

            Assert.That(() => JsonSerializer.Deserialize<FilterModel>(filterModel), Throws.TypeOf<JsonException>().With.Message.EqualTo("Invalid FilterMethod value: max"));
        }
    }
}
