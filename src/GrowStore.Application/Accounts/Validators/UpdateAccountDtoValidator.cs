using FluentValidation;
using GrowStore.Application.Accounts.DTOs;

namespace GrowStore.Application.Accounts.Validators;

public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
{
    public UpdateAccountDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("The provided email is invalid.")
            .MaximumLength(256).WithMessage("Email must be at most 256 characters long.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("The provided role is invalid.");
    }
}
