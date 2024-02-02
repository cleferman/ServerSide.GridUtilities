using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ServerSide.GridUtilities.Example.API.DatabaseContext;
using ServerSide.GridUtilities.Example.API.DatabaseContext.SeedData;
using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var options = new DbContextOptionsBuilder<StudentsContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;

var fixtures = new StudentsSeedFixtures(options);
MongoDbSeedFixtures mongoFixtures = new MongoDbSeedFixtures();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app
    .MapGet("/students", GetFilteredStudents)
    .WithName("GetStudents")
    .WithOpenApi();
app
    .MapGet("/testDocuments", GetFilteredTestDocument)
    .WithName("GetTestDocuments")
    .WithOpenApi();

app.Run();

async Task<GridResults<Student>> GetFilteredStudents([FromBody] GridRequest gridRequest)
{
    return await GetFilteredEntity<Student>(gridRequest);

    var query = fixtures.StudentsContext.Students
        .Select(gridRequest.Columns)
        .FilterBy(gridRequest.Filtering)
        .OrderBy(gridRequest.Sorting);

    var results = await query.ToListAsync();
    var totalCount = await query.CountAsync();

    return new GridResults<Student>
    {
        Results = results,
        TotalCount = totalCount
    };
}

async Task<GridResults<T>> GetFilteredEntity<T>(GridRequest gridRequest) where T : class
{
    var query = fixtures.StudentsContext.Set<T>()
        .Select(gridRequest.Columns)
        .FilterBy(gridRequest.Filtering)
        .OrderBy(gridRequest.Sorting);

    var results = await query.ToListAsync();
    var totalCount = await query.CountAsync();

    return new GridResults<T>
    {
        Results = results,
        TotalCount = totalCount
    };
}

GridResults<TestDocument> GetFilteredTestDocument([FromBody] GridRequest gridRequest)
{
    var query = mongoFixtures.mongoContext.collection.AsQueryable()
        .Select(gridRequest.Columns)
        .FilterBy(gridRequest.Filtering)
        .OrderBy(gridRequest.Sorting);

    var results = query.ToList();
    var totalCount = query.Count();

    return new GridResults<TestDocument>
    {
        Results = results,
        TotalCount = totalCount
    };
}