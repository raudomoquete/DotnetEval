using Application.Commands;
using Application.Validators;
using Microsoft.Extensions.Configuration;

namespace Application.Tests.Validators;

/// <summary>
/// Unit tests for CreateUserCommandValidator
/// </summary>
public class CreateUserCommandValidatorTests
{
    private readonly IConfiguration _configuration;
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        // Arrange: Set up configuration with regex patterns
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Validation:EmailRegex", "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$" },
            { "Validation:PasswordRegex", "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$" }
        });

        _configuration = configurationBuilder.Build();
        _validator = new CreateUserCommandValidator(_configuration);
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsValid()
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "John Doe",
            Email: "john.doe@example.com",
            Password: "SecurePass123!");

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
    public void Validate_EmptyName_ReturnsError(string? name)
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: name!,
            Email: "john.doe@example.com",
            Password: "SecurePass123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" && 
            e.ErrorMessage == "El campo de nombre no puede estar vacío");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@example.com")]
    [InlineData("invalid.email")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_InvalidEmail_ReturnsError(string? email)
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "John Doe",
            Email: email!,
            Password: "SecurePass123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Email" && 
            (e.ErrorMessage == "El correo electrónico es requerido" ||
             e.ErrorMessage == "El correo electrónico no es válido"));
    }

    [Theory]
    [InlineData("valid@example.com")]
    [InlineData("user.name@example.co.uk")]
    [InlineData("user+tag@example.com")]
    [InlineData("user_name@example-domain.com")]
    public void Validate_ValidEmail_ReturnsValid(string email)
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "John Doe",
            Email: email,
            Password: "SecurePass123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("short")] // Less than 8 characters
    [InlineData("nouppercase123!")] // No uppercase
    [InlineData("NOLOWERCASE123!")] // No lowercase
    [InlineData("NoSymbols123")] // No symbols
    [InlineData("NoNumbers!")] // No numbers
    [InlineData("onlylowercase")] // Only lowercase
    [InlineData("ONLYUPPERCASE")] // Only uppercase
    [InlineData("12345678")] // Only numbers
    [InlineData("")] // Empty
    [InlineData(null)] // Null
    public void Validate_InvalidPassword_ReturnsError(string? password)
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "John Doe",
            Email: "john.doe@example.com",
            Password: password!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            (e.ErrorMessage == "La contraseña es requerida" ||
             e.ErrorMessage == "La contraseña debe contener mayúsculas, minúsculas, símbolos y tener más de 8 caracteres"));
    }

    [Theory]
    [InlineData("SecurePass123!")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("Complex#123")]
    [InlineData("Valid$Pass1")]
    [InlineData("Test1234@")]
    public void Validate_ValidPassword_ReturnsValid(string password)
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "John Doe",
            Email: "john.doe@example.com",
            Password: password);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().NotContain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_MultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "", // Invalid
            Email: "invalid-email", // Invalid
            Password: "short"); // Invalid

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}

