using Application.Commands;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Application.Validators;

/// <summary>
/// Validator for CreateUserCommand using FluentValidation
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IConfiguration _configuration;

    public CreateUserCommandValidator(IConfiguration configuration)
    {
        _configuration = configuration;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El campo de nombre no puede estar vacío");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El correo electrónico es requerido")
            .Matches(_configuration["Validation:EmailRegex"] ?? "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$")
            .WithMessage("El correo electrónico no es válido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .Matches(_configuration["Validation:PasswordRegex"] ?? "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")
            .WithMessage("La contraseña debe contener mayúsculas, minúsculas, símbolos y tener más de 8 caracteres");
    }
}

