using Application;
using API.Middleware;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace API;

/// <summary>
/// Extension methods for configuring API services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds API services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        // Add Application layer services
        services.AddApplication();

        // Add Infrastructure layer services
        services.AddInfrastructure(configuration);

        // TODO: Add JWT Authentication configuration
        // services.AddAuthentication(...)
        // services.AddAuthorization(...)

        // TODO: Add custom middleware registration if needed

        return services;
    }

    /// <summary>
    /// Configures the HTTP request pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static WebApplication UseApiConfiguration(this WebApplication app)
    {
        // Use global exception handler middleware (must be first in pipeline)
        app.UseMiddleware<GlobalExceptionHandler>();

        // Configure Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}

