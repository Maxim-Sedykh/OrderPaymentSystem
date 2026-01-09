namespace OrderPaymentSystem.Application.DTOs.OrderItem;

public class CreateOrderItemDto
{
    public long OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
