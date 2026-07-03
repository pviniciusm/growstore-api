using FluentValidation;
using GrowStore.Application.Orders.DTOs;

namespace GrowStore.Application.Orders.Validators;

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductVariantId)
            .NotEmpty().WithMessage("ProductVariantId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}