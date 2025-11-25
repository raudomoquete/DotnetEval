using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Base API controller that provides standardized error handling methods
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Returns a problem response for a list of errors
    /// </summary>
    /// <param name="errors">List of errors to return</param>
    /// <returns>ObjectResult with ProblemDetails</returns>
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        var firstError = errors[0];
        var statusCode = firstError.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: firstError.Description);
    }

    /// <summary>
    /// Returns a problem response for a single error
    /// </summary>
    /// <param name="error">The error to return</param>
    /// <returns>ObjectResult with ProblemDetails</returns>
    protected IActionResult Problem(Error error)
    {
        return Problem(new List<Error> { error });
    }

    /// <summary>
    /// Returns a validation problem response for a list of errors
    /// </summary>
    /// <param name="errors">List of validation errors</param>
    /// <returns>ObjectResult with ValidationProblemDetails</returns>
    protected IActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();

        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(
                error.Code,
                error.Description);
        }

        return ValidationProblem(modelStateDictionary);
    }
}

