using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopScoreApi.Application.Common.Interfaces;
using TopScoreApi.Infrastructure.Persistence.DataAccess;

namespace TopScoreApi.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new KeyNotFoundException("Connection string not found");

        var dbFileName = connectionString.Replace("Data Source=", "").Trim();

        var currentDirectory = Directory.GetCurrentDirectory();
        var dbPath = Path.Combine(currentDirectory, dbFileName);

        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        connectionString = $"Data Source={dbPath}";

        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));

        services.AddScoped<IApplicationDbContext>(p => p.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}