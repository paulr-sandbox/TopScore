using Scalar.AspNetCore;
using TopScoreApi.Application;
using TopScoreApi.Infrastructure;
using TopScoreApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // await app.Services.ApplyMigrationsAsync(); TODO: Can remove since the CLI app is meant to create this
    app.MapOpenApi();

    app.MapScalarApiReference(o =>
    {
        o.WithTitle("Top Score API Docs");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
