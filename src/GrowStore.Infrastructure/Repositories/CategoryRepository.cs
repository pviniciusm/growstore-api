using GrowStore.Domain.Entities.Categories;
using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowStore.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly GrowStoreDbContext _context;

    public CategoryRepository(GrowStoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Categories.AnyAsync(c => c.Name == name);
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}