using FluentValidation;
using GrowStore.Application.Products.DTOs;
using GrowStore.Application.Shared.Rules;

namespace GrowStore.Application.Products.Validators;

public class UpdateProductVariantDtoValidator : AbstractValidator<UpdateProductVariantDto>
{
    public UpdateProductVariantDtoValidator()
    {
        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Color must be at most 50 characters long.")
            .When(x => x.Color is not null);

        RuleFor(x => x.Size)
            .MaximumLength(20).WithMessage("Size must be at most 20 characters long.")
            .When(x => x.Size is not null);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Variant price must be greater than 0.");

        RuleFor(x => x.Sku)
            .MaximumLength(50).WithMessage("SKU must be at most 50 characters long.")
            .Must(ProductValidationRules.BeAValidSku).WithMessage("SKU must contain only uppercase letters, numbers, hyphens, and underscores.")
            .When(x => !string.IsNullOrWhiteSpace(x.Sku));
    }
}
