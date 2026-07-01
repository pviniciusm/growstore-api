using FluentValidation;
using GrowStore.Application.Accounts.DTOs;
using GrowStore.Application.Shared.Rules;

namespace GrowStore.Application.Accounts.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
            .MaximumLength(512).WithMessage("New password must be at most 512 characters long.")
            .Must(AccountValidationRules.BeAStrongPassword).WithMessage("New password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.");
    }
}
