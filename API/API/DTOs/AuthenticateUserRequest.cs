namespace API.DTOs;

/// <summary>
/// Request DTO for user authentication
/// </summary>
public record AuthenticateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

