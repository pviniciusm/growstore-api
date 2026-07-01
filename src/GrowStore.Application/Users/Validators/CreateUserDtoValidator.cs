using FluentValidation;
using GrowStore.Application.Shared.Rules;
using GrowStore.Application.Users.DTOs;

namespace GrowStore.Application.Users.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
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

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("The provided role is invalid.");
    }
}
