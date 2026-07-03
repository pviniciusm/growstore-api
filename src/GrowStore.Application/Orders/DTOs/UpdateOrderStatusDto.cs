using GrowStore.Domain.Shared.Enums;

namespace GrowStore.Application.Orders.DTOs;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}