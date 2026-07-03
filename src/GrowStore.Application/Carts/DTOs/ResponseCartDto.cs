namespace GrowStore.Application.Carts.DTOs;

public class ResponseCartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<ResponseCartItemDto> Items { get; set; } = [];
    public decimal Subtotal { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
