using FluentValidation;
using GrowStore.Application.Auth.DTOs;
using GrowStore.Application.Shared.Rules;

namespace GrowStore.Application.Auth.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must be at most 150 characters long.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required.")
            .Must(UserValidationRules.BeAValidCpf).WithMessage("The provided CPF is invalid.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            .LessThan(DateTime.UtcNow).WithMessage("Birth date cannot be in the future.")
            .Must(UserValidationRules.BeAtLeast18YearsOld).WithMessage("User must be at least 18 years old.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("The provided email is invalid.")
            .MaximumLength(256).WithMessage("Email must be at most 256 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(512).WithMessage("Password must be at most 512 characters long.")
            .Must(AccountValidationRules.BeAStrongPassword).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
    }
}
