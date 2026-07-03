using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using GrowStore.Application.Orders.DTOs;
using GrowStore.Application.Orders.Interfaces;
using GrowStore.Domain.Entities.Orders;
using GrowStore.Domain.Interfaces;

namespace GrowStore.Application.Orders.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<OrderResponseDto>> CreateAsync(Guid userId, CreateOrderDto dto)
    {
        if (dto.Items.Count == 0)
            return Result<OrderResponseDto>.Failure(
                Error.Validation("Order.Validation", "Order must have at least one item."));

        var order = Order.Create(userId);

        foreach (var item in dto.Items)
        {
            var variant = await _productRepository.GetVariantByIdAsync(item.ProductVariantId);

            if (variant is null)
                return Result<OrderResponseDto>.Failure(
                    Error.NotFound("ProductVariant.NotFound",
                        $"Product variant '{item.ProductVariantId}' not found."));

            if (variant.Stock < item.Quantity)
                return Result<OrderResponseDto>.Failure(
                    Error.Validation("Order.InsufficientStock",
                        $"Insufficient stock for '{variant.Product?.Name}'. Available: {variant.Stock}, Requested: {item.Quantity}"));

            var variantDescription = $"{variant.Color ?? ""} {variant.Size ?? ""}".Trim();

            order.AddItem(
                variant.Id,
                variant.Product?.Name ?? "Unknown",
                string.IsNullOrEmpty(variantDescription) ? null : variantDescription,
                item.Quantity,
                variant.Price);

            variant.DecreaseStock(item.Quantity);
            await _productRepository.UpdateAsync(variant.Product!);
        }

        await _orderRepository.AddAsync(order);

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