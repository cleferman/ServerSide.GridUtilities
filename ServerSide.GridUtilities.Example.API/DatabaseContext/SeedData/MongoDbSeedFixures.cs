namespace ServerSide.GridUtilities.Example.API.DatabaseContext.SeedData
{
    public class MongoDbSeedFixtures
    {
        public readonly MongoDbContext mongoContext;
        public MongoDbSeedFixtures()
        {
            mongoContext = new MongoDbContext();
            mongoContext.collection.InsertOne(TestDocument.DummyData1());
            mongoContext.collection.InsertOne(TestDocument.DummyData2());
            mongoContext.collection.InsertOne(TestDocument.DummyData3());
        }
    }
}