namespace GrowStore.Application.Carts.DTOs;

public class UpdateCartItemDto
{
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
}
