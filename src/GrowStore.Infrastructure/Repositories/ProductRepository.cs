using GrowStore.Domain.Entities.Products;
using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using GrowStore.Infrastructure.Extensions;
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

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllPagedAsync(
        string? name, Guid? categoryId, decimal? minPrice, decimal? maxPrice, int page, int limit)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .AsQueryable()
            .WhereIf(!string.IsNullOrWhiteSpace(name), p => EF.Functions.Like(p.Name, $"%{name}%"))
            .WhereIf(categoryId.HasValue, p => p.CategoryId == categoryId!.Value)
            .WhereIf(minPrice.HasValue, p => p.Price >= minPrice!.Value)
            .WhereIf(maxPrice.HasValue, p => p.Price <= maxPrice!.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name);
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task<bool> SkuExistsAsync(string? sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;

        return await _context.ProductVariants.AnyAsync(v => v.Sku == sku);
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
