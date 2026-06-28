using GrowStore.Domain.Entities.Categories;
using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Products;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<ProductVariant> _variants = [];
    public IReadOnlyList<ProductVariant> Variants => _variants.AsReadOnly();

    private Product()
    {
        // Necessário para o Entity Framework
    }

    private Product(string name, string? description, decimal price, string? imageUrl, Guid categoryId)
    {
        Validate(name, price);

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Product Create(string name, string? description, decimal price, string? imageUrl, Guid categoryId)
        => new(name, description, price, imageUrl, categoryId);

    public void Update(string name, string? description, decimal price, string? imageUrl)
    {
        Validate(name, price);

        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddVariant(ProductVariant variant)
    {
        if (variant is null)
            throw new DomainException("Variant cannot be null.");

        _variants.Add(variant);
    }

    private static void Validate(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");

        if (price < 0)
            throw new DomainException("Price must be greater than or equal to zero.");
    }
}
