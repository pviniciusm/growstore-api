namespace GrowStore.Application.Products.DTOs;

public class ProductFilterDto
{
    public string? Name { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}
