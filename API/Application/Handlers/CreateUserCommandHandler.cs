using Application.Commands;
using ErrorOr;
using Infrastructure.Data.DbContext;
using Infrastructure.Data.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers;

/// <summary>
/// Handler for CreateUserCommand
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<CreateUserCommandResult>>
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public CreateUserCommandHandler(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ErrorOr<CreateUserCommandResult>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Check if email already exists
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (emailExists)
        {
            return Error.Conflict(
                code: "User.EmailAlreadyExists",
                description: "El correo ya se encuentra registrado");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email.ToLower(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email);

        return new CreateUserCommandResult(
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            Token: token
        );
    }
}

