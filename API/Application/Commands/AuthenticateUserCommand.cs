using ErrorOr;

namespace Application.Commands;

/// <summary>
/// Command for authenticating a user
/// </summary>
public record AuthenticateUserCommand(
    string Email,
    string Password
) : IRequest<ErrorOr<AuthenticateUserCommandResult>>;

/// <summary>
/// Result of AuthenticateUserCommand
/// </summary>
public record AuthenticateUserCommandResult(string Token);

