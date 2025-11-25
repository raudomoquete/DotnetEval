using Application.Commands;
using FluentValidation;

namespace Application.Validators;

/// <summary>
/// Validator for AuthenticateUserCommand using FluentValidation
/// </summary>
public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El correo electrónico es requerido")
            .EmailAddress()
            .WithMessage("El correo electrónico no es válido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida");
    }
}

