namespace GrowStore.Application.Products.DTOs;

public class UpdateProductVariantDto
{
    public string? Color { get; set; }
    public string? Size { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public string? Sku { get; set; }
}
