using GrowStore.Domain.Entities.Products;
using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowStore.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly GrowStoreDbContext _context;

    public ProductRepository(GrowStoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name);
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
