using System.Reflection;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Extension methods for configuring Application layer services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR with handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register FluentValidation behavior pipeline for MediatR
        // This will automatically validate requests before handlers execute
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // TODO: Add other application services here
        // - Mapping profiles (if using AutoMapper)
        // - Application-specific services

        return services;
    }
}

/// <summary>
/// Validation behavior for MediatR pipeline
/// Automatically validates requests using FluentValidation before handler execution
/// Converts validation errors to ErrorOr format
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
        {
            // Convert FluentValidation errors to ErrorOr errors
            var errors = failures.ConvertAll(failure =>
                Error.Validation(
                    code: failure.PropertyName ?? "ValidationError",
                    description: failure.ErrorMessage));

            // Check if TResponse is ErrorOr<T>
            if (typeof(TResponse).IsGenericType && 
                typeof(TResponse).GetGenericTypeDefinition() == typeof(ErrorOr<>))
            {
                var valueType = typeof(TResponse).GetGenericArguments()[0];
                var fromErrorsMethod = typeof(ErrorOr)
                    .GetMethod(nameof(ErrorOr.ErrorOr.From), new[] { typeof(List<Error>) })
                    ?.MakeGenericMethod(valueType);

                if (fromErrorsMethod != null)
                {
                    var errorOrResult = fromErrorsMethod.Invoke(null, new object[] { errors });
                    if (errorOrResult is TResponse response)
                    {
                        return response;
                    }
                }
            }

            // If not ErrorOr, throw exception (will be caught by GlobalExceptionHandler)
            throw new FluentValidation.ValidationException(failures);
        }

        return await next();
    }
}

