using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Carts;

public class Cart
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public List<CartItem> Items { get; private set; } = [];

    private Cart()
    {
        // Necessário para o Entity Framework
    }

    private Cart(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId is required.");

        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Cart Create(Guid userId)
        => new(userId);

    public void AddItem(Guid productVariantId, int quantity)
    {
        if (productVariantId == Guid.Empty)
            throw new DomainException("ProductVariantId is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var existingItem = Items.FirstOrDefault(i => i.ProductVariantId == productVariantId);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = CartItem.Create(Id, productVariantId, quantity);
            Items.Add(item);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItemQuantity(Guid productVariantId, int quantity)
    {
        if (productVariantId == Guid.Empty)
            throw new DomainException("ProductVariantId is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var item = Items.FirstOrDefault(i => i.ProductVariantId == productVariantId);

        if (item == null)
            throw new DomainException("Item not found in cart.");

        item.UpdateQuantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid productVariantId)
    {
        if (productVariantId == Guid.Empty)
            throw new DomainException("ProductVariantId is required.");

        var item = Items.FirstOrDefault(i => i.ProductVariantId == productVariantId);

        if (item == null)
            throw new DomainException("Item not found in cart.");

        Items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Clear()
    {
        Items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetSubtotal()
    {
        return Items.Sum(item => item.GetTotal());
    }
}
