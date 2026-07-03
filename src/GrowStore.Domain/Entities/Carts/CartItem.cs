using GrowStore.Domain.Entities.Products;
using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Carts;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Cart? Cart { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private CartItem()
    {
        // Necessário para o Entity Framework
    }

    private CartItem(Guid cartId, Guid productVariantId, int quantity)
    {
        if (cartId == Guid.Empty)
            throw new DomainException("CartId is required.");

        if (productVariantId == Guid.Empty)
            throw new DomainException("ProductVariantId is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Id = Guid.NewGuid();
        CartId = cartId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        Price = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static CartItem Create(Guid cartId, Guid productVariantId, int quantity)
        => new(cartId, productVariantId, quantity);

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("Price cannot be negative.");

        Price = price;
    }

    public decimal GetTotal()
    {
        return Price * Quantity;
    }
}
