namespace GrowStore.Domain.Entities.Orders;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string? VariantDescription { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private OrderItem() { }

    public static OrderItem Create(
        Guid orderId, Guid productVariantId, string productName,
        string? variantDescription, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new Shared.Exceptions.DomainException("Quantity must be greater than zero.");
        if (unitPrice < 0)
            throw new Shared.Exceptions.DomainException("Unit price cannot be negative.");

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductVariantId = productVariantId,
            ProductName = productName,
            VariantDescription = variantDescription,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}