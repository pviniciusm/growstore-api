using FluentValidation;
using GrowStore.Application.Orders.DTOs;

namespace GrowStore.Application.Orders.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item.");
    }
}