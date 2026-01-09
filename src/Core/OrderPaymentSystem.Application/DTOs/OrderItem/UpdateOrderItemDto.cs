namespace OrderPaymentSystem.Application.DTOs.OrderItem;

public record UpdateOrderItemDto
{
    public int ProductId { get; set; }
    public int NewQuantity { get; set; }
}
