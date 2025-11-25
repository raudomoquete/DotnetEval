namespace API.DTOs;

/// <summary>
/// Request DTO for user registration
/// </summary>
public record RegisterUserRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

