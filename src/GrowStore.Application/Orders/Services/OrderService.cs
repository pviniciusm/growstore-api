using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Application.Orders.DTOs;
using GrowStore.Application.Orders.Interfaces;
using GrowStore.Domain.Entities.Orders;
using GrowStore.Domain.Interfaces;
using FluentValidation;

namespace GrowStore.Application.Orders.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IValidator<UpdateOrderStatusDto> _updateStatusValidator;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository,
        IValidator<UpdateOrderStatusDto> updateStatusValidator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _cartRepository = cartRepository;
        _updateStatusValidator = updateStatusValidator;
    }

    public async Task<Result<OrderResponseDto>> CreateFromCartAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            return Result<OrderResponseDto>.Failure(
                Error.Validation("Order.InvalidUserId", "UserId is required."));

        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null || cart.Items.Count == 0)
            return Result<OrderResponseDto>.Failure(
                Error.Validation("Order.EmptyCart", "Cart is empty. Add items before placing an order."));

        var order = Order.Create(userId);

        foreach (var cartItem in cart.Items)
        {
            var variant = await _productRepository.GetVariantByIdAsync(cartItem.ProductVariantId);

            if (variant is null)
                return Result<OrderResponseDto>.Failure(
                    Error.NotFound("ProductVariant.NotFound",
                        $"Product variant '{cartItem.ProductVariantId}' not found."));

            if (variant.Stock < cartItem.Quantity)
                return Result<OrderResponseDto>.Failure(
                    Error.Validation("Order.InsufficientStock",
                        $"Insufficient stock for '{variant.Product?.Name}'. Available: {variant.Stock}, Requested: {cartItem.Quantity}"));

            var variantDescription = $"{variant.Color ?? ""} {variant.Size ?? ""}".Trim();

            order.AddItem(
                variant.Id,
                variant.Product?.Name ?? "Unknown",
                string.IsNullOrEmpty(variantDescription) ? null : variantDescription,
                cartItem.Quantity,
                variant.Price);

            variant.DecreaseStock(cartItem.Quantity);
            await _productRepository.UpdateAsync(variant.Product!);
        }

        await _orderRepository.AddAsync(order);
        await _cartRepository.DeleteAsync(userId);

        return Result<OrderResponseDto>.Success(MapToResponse(order));
    }

    public async Task<Result<OrderResponseDto>> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return Result<OrderResponseDto>.Failure(
                Error.NotFound("Order.NotFound", "Order not found."));

        return Result<OrderResponseDto>.Success(MapToResponse(order));
    }

    public async Task<Result<IEnumerable<OrderResponseDto>>> GetByUserIdAsync(Guid userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        var response = orders.Select(MapToResponse);
        return Result<IEnumerable<OrderResponseDto>>.Success(response);
    }

    public async Task<Result> UpdateStatusAsync(Guid orderId, UpdateOrderStatusDto dto)
    {
        var validation = await _updateStatusValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Failure(
                Error.Validation("Order.Validation",
                    string.Join(" ", validation.Errors.Select(e => e.ErrorMessage))));

        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            return Result.Failure(
                Error.NotFound("Order.NotFound", "Order not found."));

        order.UpdateStatus(dto.Status);
        await _orderRepository.UpdateAsync(order);

        return Result.Success();
    }

    private static OrderResponseDto MapToResponse(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductName,
                VariantDescription = i.VariantDescription,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}