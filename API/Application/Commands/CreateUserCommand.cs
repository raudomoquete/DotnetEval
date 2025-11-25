using ErrorOr;

namespace Application.Commands;

/// <summary>
/// Command for creating a new user
/// </summary>
public record CreateUserCommand(
    string Name,
    string Email,
    string Password
) : IRequest<ErrorOr<CreateUserCommandResult>>;

/// <summary>
/// Result of CreateUserCommand
/// </summary>
public record CreateUserCommandResult(
    Guid Id,
    string Name,
    string Email,
    string Token
);

