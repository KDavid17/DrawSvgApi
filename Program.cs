using Models;
using System.Text.Json;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.AllowAnyHeader();
                          policy.WithMethods("GET", "PUT");
                          policy.WithOrigins("http://localhost:4200");
                      });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/svg-json", () =>
{
    string filePath = "Data//SvgDimensions.json";

    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException();
    }

    string jsonString = File.ReadAllText(filePath);

    try
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var data = JsonSerializer.Deserialize<SvgDimensionsDTO>(jsonString, options);

        return Results.Ok(data);
    }
    catch (JsonException ex)
    {
        throw new JsonException("Error while deserializing", ex);
    }
});

app.MapPut("/svg-json", (SvgDimensionsDTO dimensions) =>
{
    string filePath = "Data//SvgDimensions.json";

    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException();
    }

    try
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonString = JsonSerializer.Serialize(dimensions, options);

        File.WriteAllText(filePath, jsonString);

        return Results.Ok("File has been updated successfully.");
    }
    catch (JsonException ex)
    {
        throw new JsonException("Error while serializing", ex);
    }
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

namespace Models
{
    public record SvgDimensionsDTO(int Height, int Width);
}