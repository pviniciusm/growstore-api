namespace GrowStore.Application.Carts.DTOs;

public class ResponseCartItemDto
{
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }
}
