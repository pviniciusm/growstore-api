using FluentValidation;
using GrowStore.Application.Carts.DTOs;

namespace GrowStore.Application.Carts.Validators;

public class CreateCartItemDtoValidator : AbstractValidator<CreateCartItemDto>
{
    public CreateCartItemDtoValidator()
    {
        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithMessage("Product Variant ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
