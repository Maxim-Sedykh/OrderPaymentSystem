namespace OrderPaymentSystem.Application.DTOs.Basket;

public record CreateBasketItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
