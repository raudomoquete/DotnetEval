using Application.Commands;
using ErrorOr;
using Infrastructure.Data.DbContext;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers;

/// <summary>
/// Handler for AuthenticateUserCommand
/// </summary>
public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, ErrorOr<AuthenticateUserCommandResult>>
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticateUserCommandHandler(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ErrorOr<AuthenticateUserCommandResult>> Handle(
        AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (user == null)
        {
            return Error.NotFound(
                code: "User.NotFound",
                description: "Usuario no encontrado");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Error.Validation(
                code: "User.InvalidCredentials",
                description: "Correo o contraseña inválidos");
        }

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email);

        return new AuthenticateUserCommandResult(Token: token);
    }
}

