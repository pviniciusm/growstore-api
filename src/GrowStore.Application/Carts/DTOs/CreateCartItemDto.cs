namespace GrowStore.Application.Carts.DTOs;

public class CreateCartItemDto
{
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
}
