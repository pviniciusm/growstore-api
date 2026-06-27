using GrowStore.Domain.Shared.Exceptions;

namespace GrowStore.Domain.Entities.Categories;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Category()
    {
        // Necessário para o Entity Framework
    }

    private Category(string name, string? description)
    {
        Validate(name);

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Category Create(string name, string? description) => new(name, description);

    public void Update(string name, string? description)
    {
        Validate(name);

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");
    }
}