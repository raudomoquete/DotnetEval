namespace Infrastructure.External;

/// <summary>
/// Post model from JsonPlaceholder API
/// </summary>
public record JsonPlaceholderPost
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}

/// <summary>
/// Request model for creating a post in JsonPlaceholder API
/// </summary>
public record CreateJsonPlaceholderPostRequest
{
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}

