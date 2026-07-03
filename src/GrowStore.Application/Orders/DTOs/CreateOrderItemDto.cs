namespace GrowStore.Application.Orders.DTOs;

public class CreateOrderItemDto
{
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
}