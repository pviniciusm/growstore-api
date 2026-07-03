using GrowStore.Domain.Entities.Carts;

namespace GrowStore.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId);
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
    Task DeleteAsync(Guid userId);
    Task RemoveItemAsync(Guid cartId, Guid productVariantId);
    Task<bool> ProductVariantExistsAsync(Guid productVariantId);
    Task<decimal> GetProductVariantPriceAsync(Guid productVariantId);
}
