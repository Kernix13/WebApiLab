using System.Text.Json;
using WebApiLab.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

string jsonFile = File.ReadAllText("./Resources/64KB.json");

var jsonData = JsonSerializer.Deserialize<List<Person>>(
    jsonFile,
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

app.MapGet(pattern: "/people", handler: () => jsonData)
    .WithName(endpointName: "GetPeople")
    .WithOpenApi()
    .Produces<List<Person>>(statusCode: StatusCodes.Status200OK);

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
