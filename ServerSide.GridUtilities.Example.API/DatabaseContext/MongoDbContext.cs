using Mongo2Go;
using MongoDB.Driver;
using ServerSide.GridUtilities.Example.API.DatabaseContext.SeedData;

namespace ServerSide.GridUtilities.Example.API.DatabaseContext;

public class MongoDbContext
{
    public readonly IMongoCollection<TestDocument> collection;

    public MongoDbContext()
    {
        var runner = MongoDbRunner.Start();
        MongoClient client = new MongoClient(runner.ConnectionString);
        var database = client.GetDatabase("IntegrationTest");
        collection = database.GetCollection<TestDocument>("TestCollection");
    }
}