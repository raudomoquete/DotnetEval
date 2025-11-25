using API.DTOs;
using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for user registration and authentication
/// </summary>
public class UsersController : ApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registered user information with JWT token</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            Name: request.Name,
            Email: request.Email,
            Password: request.Password
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            value => Ok(new RegisterUserResponse
            {
                Id = value.Id,
                Name = value.Name,
                Email = value.Email,
                Token = value.Token
            }),
            errors => Problem(errors));
    }

    /// <summary>
    /// Authenticate a user
    /// </summary>
    /// <param name="request">User credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token for authenticated user</returns>
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(AuthenticateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Authenticate(
        [FromBody] AuthenticateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AuthenticateUserCommand(
            Email: request.Email,
            Password: request.Password
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            value => Ok(new AuthenticateUserResponse
            {
                Token = value.Token
            }),
            errors => Problem(errors));
    }
}

