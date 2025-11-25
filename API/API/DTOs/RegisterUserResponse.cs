namespace API.DTOs;

/// <summary>
/// Response DTO for user registration
/// </summary>
public record RegisterUserResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}

