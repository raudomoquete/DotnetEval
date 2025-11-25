namespace API.DTOs;

/// <summary>
/// Response DTO for user authentication
/// </summary>
public record AuthenticateUserResponse
{
    public string Token { get; init; } = string.Empty;
}

