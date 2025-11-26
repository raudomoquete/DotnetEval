using Application.Commands;
using Application.Handlers;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests.Handlers;

/// <summary>
/// Unit tests for CreateUserCommandHandler
/// </summary>
public class CreateUserCommandHandlerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
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
        _handler = new CreateUserCommandHandler(
            _context,
            _mockPasswordHasher.Object,
            _mockJwtTokenService.Object);
    }

    [Fact]
    public async Task Handle_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "John Doe",
            Email: "john.doe@example.com",
            Password: "SecurePass123!");

        var expectedPasswordHash = "hashed_password_123";
        var expectedToken = "jwt_token_123";

        _mockPasswordHasher
            .Setup(x => x.HashPassword(command.Password))
            .Returns(expectedPasswordHash);

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(It.IsAny<Guid>(), command.Email.ToLower()))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Email.Should().Be(command.Email.ToLower());
        result.Value.Token.Should().Be(expectedToken);
        result.Value.Id.Should().NotBeEmpty();

        // Verify user was saved to database
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email.ToLower());
        savedUser.Should().NotBeNull();
        savedUser!.Name.Should().Be(command.Name);
        savedUser.PasswordHash.Should().Be(expectedPasswordHash);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsConflictError()
    {
        // Arrange
        var existingEmail = "existing@example.com";
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = existingEmail,
            PasswordHash = "existing_hash",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var command = new CreateUserCommand(
            Name: "New User",
            Email: existingEmail,
            Password: "NewPass123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Conflict);
        result.FirstError.Code.Should().Be("User.EmailAlreadyExists");
        result.FirstError.Description.Should().Be("El correo ya se encuentra registrado");

        // Verify password hasher was not called
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmailCaseInsensitive_ReturnsConflictError()
    {
        // Arrange
        var existingEmail = "existing@example.com";
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = existingEmail.ToLower(),
            PasswordHash = "existing_hash",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var command = new CreateUserCommand(
            Name: "New User",
            Email: existingEmail.ToUpper(), // Different case
            Password: "NewPass123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Conflict);
        result.FirstError.Code.Should().Be("User.EmailAlreadyExists");
    }

    [Fact]
    public async Task Handle_ValidUser_EmailStoredInLowercase()
    {
        // Arrange
        var command = new CreateUserCommand(
            Name: "John Doe",
            Email: "John.Doe@Example.COM", // Mixed case
            Password: "SecurePass123!");

        _mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns("jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Email.Should().Be(command.Email.ToLower());

        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email.ToLower());
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be(command.Email.ToLower());
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}

