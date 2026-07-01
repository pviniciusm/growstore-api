using FluentValidation;
using GrowStore.Application.Accounts.DTOs;
using GrowStore.Application.Shared.Rules;

namespace GrowStore.Application.Accounts.Validators;

public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
{
    public CreateAccountDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("The provided email is invalid.")
            .MaximumLength(256).WithMessage("Email must be at most 256 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(512).WithMessage("Password must be at most 512 characters long.")
            .Must(AccountValidationRules.BeAStrongPassword).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("The provided role is invalid.");
    }
}
