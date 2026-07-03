using GrowStore.Domain.Shared.Enums;

namespace GrowStore.Domain.Entities.Orders;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(i => i.TotalPrice);

    private Order() { }

    public static Order Create(Guid userId)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.PENDING,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void AddItem(Guid productVariantId, string productName,
        string? variantDescription, int quantity, decimal unitPrice)
    {
        _items.Add(OrderItem.Create(Id, productVariantId, productName,
            variantDescription, quantity, unitPrice));
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}