using Infrastructure.Services;

namespace Infrastructure.Tests.Services;

/// <summary>
/// Unit tests for PasswordHasher service using BCrypt
/// </summary>
public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Should().StartWith("$2a$"); // BCrypt hash prefix
        hashedPassword.Length.Should().Be(60); // BCrypt hash length
    }

    [Fact]
    public void HashPassword_DifferentPasswords_ReturnsDifferentHashes()
    {
        // Arrange
        var password1 = "Password123!";
        var password2 = "DifferentPassword456!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password1);
        var hash2 = _passwordHasher.HashPassword(password2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void HashPassword_SamePassword_ReturnsDifferentHashes()
    {
        // Arrange
        var password = "SamePassword123!";

        // Act
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // Assert
        // BCrypt generates different hashes each time (salt is random)
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(password, hashedPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "SecurePassword123!";
        var incorrectPassword = "WrongPassword123!";
        var hashedPassword = _passwordHasher.HashPassword(correctPassword);

        // Act
        var result = _passwordHasher.VerifyPassword(incorrectPassword, hashedPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_EmptyPassword_ReturnsFalse()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hashedPassword = _passwordHasher.HashPassword(password);

        // Act
        var result = _passwordHasher.VerifyPassword(string.Empty, hashedPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("MySecure@Pass1")]
    [InlineData("Complex#Password99")]
    [InlineData("Test$Pass123")]
    public void HashAndVerifyPassword_VariousPasswords_WorksCorrectly(string password)
    {
        // Arrange & Act
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(password, hashedPassword);

        // Assert
        result.Should().BeTrue();
    }
}

