using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/students", GetFilteredStudents)
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

async Task<GridResults<Student>> GetFilteredStudents([FromBody] GridRequest gridRequest)
{
    var query = fixtures.StudentsContext.Students
        .SelectColumns(gridRequest.Columns)
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