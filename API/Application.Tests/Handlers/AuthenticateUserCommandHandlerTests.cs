using Application.Commands;
using Application.Handlers;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests.Handlers;

/// <summary>
/// Unit tests for AuthenticateUserCommandHandler
/// </summary>
public class AuthenticateUserCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly AuthenticateUserCommandHandler _handler;

    public AuthenticateUserCommandHandlerTests()
    {
        // Arrange: Set up InMemory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        // Arrange: Set up mocks
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        // Arrange: Create handler with dependencies
        _handler = new AuthenticateUserCommandHandler(
            _context,
            _mockPasswordHasher.Object,
            _mockJwtTokenService.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var command = new AuthenticateUserCommand(
            Email: "john.doe@example.com",
            Password: "ValidPassword123!");

        var expectedToken = "jwt_token_123";

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(user.Id, user.Email))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be(expectedToken);

        _mockJwtTokenService.Verify(
            x => x.GenerateToken(user.Id, user.Email),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new AuthenticateUserCommand(
            Email: "nonexistent@example.com",
            Password: "SomePassword123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
        result.FirstError.Code.Should().Be("User.NotFound");
        result.FirstError.Description.Should().Be("Usuario no encontrado");

        // Verify password hasher was not called
        _mockPasswordHasher.Verify(
            x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ReturnsValidationError()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var command = new AuthenticateUserCommand(
            Email: "john.doe@example.com",
            Password: "WrongPassword123!");

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("User.InvalidCredentials");
        result.FirstError.Description.Should().Be("Correo o contraseña inválidos");

        // Verify JWT service was not called
        _mockJwtTokenService.Verify(
            x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_EmailCaseInsensitive_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var command = new AuthenticateUserCommand(
            Email: "John.Doe@Example.COM", // Different case
            Password: "ValidPassword123!");

        var expectedToken = "jwt_token_123";

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(user.Id, user.Email))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Token.Should().Be(expectedToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}

