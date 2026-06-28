using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Products;

public class ProductVariant
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string? Color { get; private set; }
    public string? Size { get; private set; }
    public int Stock { get; private set; }
    public decimal Price { get; private set; }
    public string? Sku { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProductVariant()
    {
        // Necessário para o Entity Framework
    }

    private ProductVariant(Guid productId, string? color, string? size, int stock, decimal price, string? sku)
    {
        Validate(stock, price);

        Id = Guid.NewGuid();
        ProductId = productId;
        Color = color;
        Size = size;
        Stock = stock;
        Price = price;
        Sku = sku;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ProductVariant Create(Guid productId, string? color, string? size, int stock, decimal price, string? sku)
        => new(productId, color, size, stock, price, sku);

    public void Update(string? color, string? size, int stock, decimal price, string? sku)
    {
        Validate(stock, price);

        Color = color;
        Size = size;
        Stock = stock;
        Price = price;
        Sku = sku;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (Stock < quantity)
            throw new DomainException("Insufficient stock.");

        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Stock += quantity;
    }

    private static void Validate(int stock, decimal price)
    {
        if (stock < 0)
            throw new DomainException("Stock cannot be negative.");

        if (price < 0)
            throw new DomainException("Price must be greater than or equal to zero.");
    }
}
