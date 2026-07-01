using FluentValidation;
using GrowStore.Application.Products.DTOs;
using GrowStore.Application.Shared.Rules;

namespace GrowStore.Application.Products.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must be at most 200 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must be at most 1000 characters long.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must be at most 500 characters long.")
            .Must(ProductValidationRules.BeAValidUrl).WithMessage("The provided Image URL is invalid.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }
}
