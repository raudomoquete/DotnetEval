namespace API.DTOs;

/// <summary>
/// Request DTO for creating a post
/// </summary>
public record CreatePostRequest
{
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}

