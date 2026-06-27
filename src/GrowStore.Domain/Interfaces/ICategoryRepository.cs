using GrowStore.Domain.Entities.Categories;

namespace GrowStore.Domain.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<bool> NameExistsAsync(string name);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
}