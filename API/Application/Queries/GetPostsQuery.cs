using ErrorOr;

namespace Application.Queries;

/// <summary>
/// Query for getting all posts from external API
/// </summary>
public record GetPostsQuery() : IRequest<ErrorOr<GetPostsQueryResult>>;

/// <summary>
/// Result of GetPostsQuery
/// </summary>
public record GetPostsQueryResult(List<PostDto> Posts);

/// <summary>
/// Post DTO for API responses
/// </summary>
public record PostDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}

