using GrowStore.Domain.Entities.Products;

namespace GrowStore.Domain.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<bool> NameExistsAsync(string name);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> CategoryExistsAsync(Guid categoryId);
}
