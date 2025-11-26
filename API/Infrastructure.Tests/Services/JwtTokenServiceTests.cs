using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Tests.Services;

/// <summary>
/// Unit tests for JwtTokenService
/// </summary>
public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_ValidInput_ReturnsValidJwtToken()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var service = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var token = service.GenerateToken(userId, email);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts: header.payload.signature
    }

    [Fact]
    public void GenerateToken_ValidInput_TokenContainsUserIdClaim()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var service = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var token = service.GenerateToken(userId, email);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Assert
        jwtToken.Claims.Should().Contain(c => 
            c.Type == ClaimTypes.NameIdentifier && 
            c.Value == userId.ToString());
    }

    [Fact]
    public void GenerateToken_ValidInput_TokenContainsEmailClaim()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var service = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var token = service.GenerateToken(userId, email);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Assert
        jwtToken.Claims.Should().Contain(c => 
            c.Type == ClaimTypes.Email && 
            c.Value == email);
    }

    [Fact]
    public void GenerateToken_ValidInput_TokenHasExpiration()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var service = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var token = service.GenerateToken(userId, email);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Assert
        jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
        jwtToken.ValidTo.Should().BeCloseTo(
            DateTime.UtcNow.AddMinutes(60), 
            TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GenerateToken_DifferentUsers_GeneratesDifferentTokens()
    {
        // Arrange
        var configuration = CreateConfiguration();
        var service = new JwtTokenService(configuration);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var token1 = service.GenerateToken(userId1, "user1@example.com");
        var token2 = service.GenerateToken(userId2, "user2@example.com");

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GenerateToken_MissingSecretKey_ThrowsException()
    {
        // Arrange
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            new JwtTokenService(configuration));
        
        exception.Message.Should().Be("JWT SecretKey is not configured");
    }

    [Fact]
    public void GenerateToken_CustomExpiration_UsesCustomValue()
    {
        // Arrange
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:SecretKey", "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForHS256!" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpirationMinutes", "30" }
        });

        var configuration = configurationBuilder.Build();
        var service = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();

        // Act
        var token = service.GenerateToken(userId, "test@example.com");
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Assert
        jwtToken.ValidTo.Should().BeCloseTo(
            DateTime.UtcNow.AddMinutes(30), 
            TimeSpan.FromMinutes(1));
    }

    private static IConfiguration CreateConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:SecretKey", "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForHS256!" },
            { "Jwt:Issuer", "EvaluacionDotnet" },
            { "Jwt:Audience", "EvaluacionDotnet" },
            { "Jwt:ExpirationMinutes", "60" }
        });

        return configurationBuilder.Build();
    }
}

