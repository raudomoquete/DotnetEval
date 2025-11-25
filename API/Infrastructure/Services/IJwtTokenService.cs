namespace Infrastructure.Services;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="email">User email</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(Guid userId, string email);
}

