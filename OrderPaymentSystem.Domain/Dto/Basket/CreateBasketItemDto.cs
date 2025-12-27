namespace OrderPaymentSystem.Domain.Dto.Basket;

public record CreateBasketItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
