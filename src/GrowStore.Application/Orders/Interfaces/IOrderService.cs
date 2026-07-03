using GrowStore.Application.Common.Results;
using GrowStore.Application.Orders.DTOs;

namespace GrowStore.Application.Orders.Interfaces;

public interface IOrderService
{
    Task<Result<OrderResponseDto>> CreateAsync(Guid userId, CreateOrderDto dto);
    Task<Result<OrderResponseDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<OrderResponseDto>>> GetByUserIdAsync(Guid userId);
    Task<Result> UpdateStatusAsync(Guid orderId, UpdateOrderStatusDto dto);
}