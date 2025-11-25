using ErrorOr;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace API.Middleware;

/// <summary>
/// Global exception handler middleware that catches all unhandled exceptions
/// and converts them to proper HTTP responses
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "An error occurred while processing your request",
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        // Handle specific exception types
        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Title = "Validation failed";
                response.Detail = "One or more validation errors occurred";
                
                var errors = validationEx.Errors
                    .Select(e => Error.Validation(
                        code: e.PropertyName ?? "ValidationError",
                        description: e.ErrorMessage))
                    .ToList();

                response.Extensions.Add("errors", errors);
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Title = "Invalid argument";
                response.Detail = argEx.Message;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Title = "Unauthorized";
                response.Detail = "You are not authorized to perform this action";
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Status = (int)HttpStatusCode.NotFound;
                response.Title = "Resource not found";
                response.Detail = exception.Message;
                break;

            default:
                // Keep the default 500 status for unknown exceptions
                response.Detail = "An unexpected error occurred. Please try again later.";
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Problem details model for error responses
/// </summary>
public class ProblemDetails
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public Dictionary<string, object> Extensions { get; set; } = new();
}

