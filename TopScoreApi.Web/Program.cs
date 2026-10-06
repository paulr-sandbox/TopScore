using Scalar.AspNetCore;
using TopScoreApi.Application;
using TopScoreApi.Infrastructure;
using TopScoreApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.Services.ApplyMigrationsAsync();
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
