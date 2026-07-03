namespace GrowStore.Application.Orders.DTOs;

public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = [];
}