using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
        // TODO: Uncomment when ApplicationDbContext is created
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseInMemoryDatabase("EvaluacionDotnetDb"));

        // TODO: Add other infrastructure services here
        // - Password hashing service (BCrypt)
        // - JWT token service
        // - External API clients
        // - Repositories (if needed)

        return services;
    }
}

