using ErrorOr;

namespace Application.Commands;

/// <summary>
/// Command for creating a post in external API
/// </summary>
public record CreatePostCommand(
    int UserId,
    string Title,
    string Body
) : IRequest<ErrorOr<CreatePostCommandResult>>;

/// <summary>
/// Result of CreatePostCommand
/// </summary>
public record CreatePostCommandResult(
    int Id,
    int UserId,
    string Title,
    string Body
);

