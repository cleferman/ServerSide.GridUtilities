using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid;
using ServerSide.GridUtilities.Grid.Constants;
using System.Text.Json;

namespace ServerSide.GridUtilities.Tests
{
    public class SortTypeTests
    {
        [Test]
        public void SortTypeSerialization_AllMembers_Success()
        {
            foreach (SortType sortType in Enum.GetValues(typeof(SortType)))
            {
                var json = JsonSerializer.Serialize(sortType);
                Assert.That(json.Equals($"\"{sortType.GetDescription()}\""));
            }
        }

        [Test]
        public void SortTypeDeSerialization_AllMembers_Success()
        {
            foreach (SortType sortType in Enum.GetValues(typeof(SortType)))
            {
                var sortModel = new SortModel
                {
                    ColName = "Name",
                    Sort = sortType
                };

                var model = JsonSerializer.Deserialize<FilterModel>(JsonSerializer.Serialize(sortModel));

                Assert.IsNotNull(model);
                Assert.That(sortModel.Sort == sortType);
            }
        }

        [Test]
        public void FilterTypeDeSerialization_Descc_InvalidFilterType()
        {
            var sortModel = "{\"ColName\":\"Name\",\"Sort\":\"descc\"}";

            Assert.That(() => JsonSerializer.Deserialize<SortModel>(sortModel), Throws.TypeOf<JsonException>().With.Message.EqualTo("Invalid SortType value: descc"));
        }
    }
}
