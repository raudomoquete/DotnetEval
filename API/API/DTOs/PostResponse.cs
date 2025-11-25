namespace API.DTOs;

/// <summary>
/// Response DTO for post data
/// </summary>
public record PostResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}

