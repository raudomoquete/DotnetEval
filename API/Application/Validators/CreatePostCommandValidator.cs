using Application.Commands;
using FluentValidation;

namespace Application.Validators;

/// <summary>
/// Validator for CreatePostCommand using FluentValidation
/// </summary>
public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("El UserId debe ser mayor que 0");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("El título es requerido")
            .MaximumLength(200)
            .WithMessage("El título no puede exceder 200 caracteres");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("El cuerpo del post es requerido")
            .MaximumLength(5000)
            .WithMessage("El cuerpo del post no puede exceder 5000 caracteres");
    }
}

