using Application.Commands;
using Application.Validators;

namespace Application.Tests.Validators;

/// <summary>
/// Unit tests for AuthenticateUserCommandValidator
/// </summary>
public class AuthenticateUserCommandValidatorTests
{
    private readonly AuthenticateUserCommandValidator _validator;

    public AuthenticateUserCommandValidatorTests()
    {
        _validator = new AuthenticateUserCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = new AuthenticateUserCommand(
            Email: "user@example.com",
            Password: "ValidPassword123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyEmail_ReturnsError(string? email)
    {
        // Arrange
        var command = new AuthenticateUserCommand(
            Email: email!,
            Password: "ValidPassword123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Email" && 
            e.ErrorMessage == "El correo electrónico es requerido");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@example.com")]
    public void Validate_InvalidEmailFormat_ReturnsError(string email)
    {
        // Arrange
        var command = new AuthenticateUserCommand(
            Email: email,
            Password: "ValidPassword123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Email" && 
            e.ErrorMessage == "El correo electrónico no es válido");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyPassword_ReturnsError(string? password)
    {
        // Arrange
        var command = new AuthenticateUserCommand(
            Email: "user@example.com",
            Password: password!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage == "La contraseña es requerida");
    }

    [Theory]
    [InlineData("valid@example.com")]
    [InlineData("user.name@example.co.uk")]
    [InlineData("user+tag@example.com")]
    public void Validate_ValidEmailFormat_ReturnsValid(string email)
    {
        // Arrange
        var command = new AuthenticateUserCommand(
            Email: email,
            Password: "AnyPassword123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain(e => e.PropertyName == "Email");
    }
}

