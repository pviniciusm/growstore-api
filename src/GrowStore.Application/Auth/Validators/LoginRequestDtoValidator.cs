using FluentValidation;
using GrowStore.Application.Auth.DTOs;

namespace GrowStore.Application.Auth.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("The provided email is invalid.")
            .MaximumLength(256).WithMessage("Email must be at most 256 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
