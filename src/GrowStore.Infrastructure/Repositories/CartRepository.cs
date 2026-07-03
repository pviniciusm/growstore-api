using GrowStore.Domain.Entities.Carts;
using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrowStore.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly GrowStoreDbContext _context;

    public CartRepository(GrowStoreDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.ProductVariant)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<bool> ExistsAsync(Guid userId)
    {
        return await _context.Carts.AnyAsync(c => c.UserId == userId);
    }

    public async Task AddAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cart cart)
    {
        var deletedCount = await _context.CartItems
            .Where(ci => ci.CartId == cart.Id)
            .ExecuteDeleteAsync();

        foreach (var newItem in cart.Items)
        {
            await _context.CartItems.AddAsync(newItem);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveItemAsync(Guid cartId, Guid productVariantId)
    {
        var deletedCount = await _context.CartItems
            .Where(ci => ci.CartId == cartId && ci.ProductVariantId == productVariantId)
            .ExecuteDeleteAsync();
    }

    public async Task<bool> ProductVariantExistsAsync(Guid productVariantId)
    {
        return await _context.ProductVariants.AnyAsync(pv => pv.Id == productVariantId);
    }

    public async Task<decimal> GetProductVariantPriceAsync(Guid productVariantId)
    {
        var variant = await _context.ProductVariants
            .FirstOrDefaultAsync(pv => pv.Id == productVariantId);

        return variant?.Price ?? 0;
    }
}
