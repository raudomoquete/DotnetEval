using Infrastructure.Data.DbContext;
using Infrastructure.External;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// Extension methods for configuring Infrastructure layer services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add InMemory Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("EvaluacionDotnetDb"));

        // Add infrastructure services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Add HTTP Client for external API
        services.AddHttpClient<IJsonPlaceholderClient, JsonPlaceholderClient>(client =>
        {
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}

