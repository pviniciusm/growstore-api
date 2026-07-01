using FluentValidation;
using GrowStore.Application.Addresses.DTOs;
using GrowStore.Application.Shared.Rules;

namespace GrowStore.Application.Addresses.Validators;

public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(200).WithMessage("Street must be at most 200 characters long.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City must be at most 100 characters long.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(100).WithMessage("State must be at most 100 characters long.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required.")
            .MaximumLength(20).WithMessage("Zip code must be at most 20 characters long.")
            .Must(AddressValidationRules.BeAValidZipCode).WithMessage("The provided zip code is invalid.");

        RuleFor(x => x.Reference)
            .MaximumLength(100).WithMessage("Reference must be at most 100 characters long.")
            .When(x => x.Reference is not null);
    }
}
