namespace GrowStore.Application.Products.DTOs;

public class ResponseProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
