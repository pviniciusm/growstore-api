using GrowStore.Domain.Entities.Products;

namespace GrowStore.Domain.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAllPagedAsync(
        string? name,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int limit);
    Task<bool> NameExistsAsync(string name);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> CategoryExistsAsync(Guid categoryId);
    Task<bool> SkuExistsAsync(string? sku);
    Task<ProductVariant?> GetVariantByIdAsync(Guid variantId);
}
