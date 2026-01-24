namespace OrderPaymentSystem.Application.DTOs.OrderItem;

public class OrderItemDto
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal ItemTotalSum { get; set; }
    public decimal ProductPrice { get; set; }
}
